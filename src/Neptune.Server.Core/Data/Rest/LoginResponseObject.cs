namespace Neptune.Server.Core.Data.Rest;

public record LoginResponseObject(string? Token, string? RefreshToken, DateTime? RefreshTokenExpire, string? Message, bool IsSuccess);
