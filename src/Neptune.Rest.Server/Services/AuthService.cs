using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Neptune.Core.Utils;
using Neptune.Database.Core.Interfaces.DataAccess;
using Neptune.Rest.Server.Entities;
using Neptune.Rest.Server.Interfaces;
using Neptune.Server.Core.Data.Config;
using Neptune.Server.Core.Data.Cryptography;
using Neptune.Server.Core.Data.Rest;

namespace Neptune.Rest.Server.Services;

public class AuthService : IAuthService
{
    private readonly ILogger _logger;

    private readonly IDataAccess<UserEntity> _userDataAccess;

    private readonly NeptuneServerConfig _neptuneServerConfig;

    public AuthService(
        ILogger<AuthService> logger, IDataAccess<UserEntity> userDataAccess, NeptuneServerConfig neptuneServerConfig
    )
    {
        _logger = logger;
        _userDataAccess = userDataAccess;
        _neptuneServerConfig = neptuneServerConfig;
    }


    private string GenerateJwtToken(UserEntity user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, user.GetFullName())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_neptuneServerConfig.JwtAuth.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _neptuneServerConfig.JwtAuth.Issuer,
            audience: _neptuneServerConfig.JwtAuth.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_neptuneServerConfig.JwtAuth.ExpirationInMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<LoginResponseObject> LoginAsync(LoginRequestObject loginRequest)
    {
        UserEntity userEntity = null;

        if (loginRequest.Username == null && loginRequest.Email == null)
        {
            _logger.LogWarning("Login failed: Username and email are both null.");

            return new LoginResponseObject(null, null, null, "Login failed: Username and email are both null.", false);
        }

        if (loginRequest.Username != null)
        {
            userEntity = await _userDataAccess.QuerySingleAsync(s => s.Username == loginRequest.Username);
        }
        else if (loginRequest.Email != null)
        {
            userEntity = await _userDataAccess.QuerySingleAsync(s => s.Email == loginRequest.Email);
        }

        if (userEntity == null)
        {
            _logger.LogWarning("Login failed: User not found.");

            return new LoginResponseObject(null, null, null, "Login failed: User not found.", false);
        }

        var cleanedPassword = userEntity.PasswordHash.Replace("hash:", "");
        var passwordHash = cleanedPassword.Split(":")[0];
        var salt = cleanedPassword.Split(":")[1];

        var isOk = HashUtils.VerifyPassword(userEntity.PasswordHash, passwordHash, salt);

        if (!isOk)
        {
            _logger.LogWarning("Login failed: Invalid password.");

            return new LoginResponseObject(null, null, null, "Login failed: Invalid password.", false);
        }

        var token = GenerateJwtToken(userEntity);
        var refreshToken = HashUtils.GenerateRandomRefreshToken(64);

        userEntity.RefreshToken = refreshToken;
        userEntity.RefreshTokenExpiry = DateTime.UtcNow.AddDays(_neptuneServerConfig.JwtAuth.RefreshTokenExpiryDays);

        await _userDataAccess.UpdateAsync(userEntity);


        _logger.LogInformation("Login successful for user: {Username}", userEntity.Username);

        return new LoginResponseObject(
            token,
            refreshToken,
            userEntity.RefreshTokenExpiry,
            null,
            true
        );
    }

    public async Task<RegisterResponseObject> RegisterAsync(RegisterRequestObject registerRequest)
    {
        var userEntity = new UserEntity();

        userEntity.Username = registerRequest.Username;

        var passwordHash = HashUtils.HashPassword(registerRequest.Password);
        userEntity.PasswordHash = "hash:" + passwordHash.Hash + ":" + passwordHash.Salt;


        userEntity.Email = registerRequest.Email;


        var existingUser = await _userDataAccess.QuerySingleAsync(s => s.Username == registerRequest.Username);

        if (existingUser != null)
        {
            _logger.LogWarning("Registration failed: User already exists.");

            return new RegisterResponseObject()
            {
                IsSuccess = false,
                Message = "User already exists."
            };
        }

        var enc = new NeptuneEncryptorX25519();

        enc.Generate();


        var privateKey = enc.ExportPrivateKeyBase64();
        var publicKey = enc.ExportPublicKeyBase64();

        userEntity.PublicKey = publicKey;
        userEntity.NodeHostName = _neptuneServerConfig.NodeName;

        var createdUser = await _userDataAccess.InsertAsync(userEntity);


        var login = await LoginAsync(
            new LoginRequestObject(registerRequest.Username, registerRequest.Email, registerRequest.Password)
        );

        return new RegisterResponseObject()
        {
            IsSuccess = true,
            PublicKey = publicKey,
            PrivateKey = privateKey,
            FullNodeId = createdUser.GetFullName(),
            LoginResponse = login
        };
    }
}
