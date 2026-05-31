namespace Studi.BLL.Models.Auth;

public sealed record AccessTokenResult(string Token, DateTime ExpiresAtUtc);
