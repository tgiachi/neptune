using System.Security.Cryptography;
using Neptune.Core.Extensions;

namespace Neptune.Server.Core.Data.Config.Sections;

public class JwtAuthConfig
{
    public string Issuer { get; set; } = "Neptune";
    public string Audience { get; set; } = "Neptune";
    public string Secret { get; set; } = RandomNumberGenerator.GetBytes(128).ToBase64();

    public int ExpirationInMinutes { get; set; } = 60 * 24 * 31; // 31 day
}
