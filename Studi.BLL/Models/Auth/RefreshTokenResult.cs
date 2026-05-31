namespace Studi.BLL.Models.Auth;

public sealed record RefreshTokenResult(string Token, string TokenHash, DateTime ExpiresAtUtc);
