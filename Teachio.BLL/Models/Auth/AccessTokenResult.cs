namespace Teachio.BLL.Models.Auth;

public sealed record AccessTokenResult(string Token, DateTime ExpiresAtUtc);
