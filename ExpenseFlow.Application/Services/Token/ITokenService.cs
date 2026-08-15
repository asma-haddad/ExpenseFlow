namespace ExpenseFlow.Application.Services.Token;

public interface ITokenService
{
    Task<TokenDto> IssueTokensAsync(Guid userId, string? ip, string? userAgent, bool isDevice);
    Task<TokenDto> RefreshAsync(string refreshToken, string? ip, string? userAgent, bool isDevice);
    Task RevokeAsync(string refreshToken, string? ip, string? reason = null);
}