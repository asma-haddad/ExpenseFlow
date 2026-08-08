using ExpenseFlow.Domain.Model.User;

namespace ExpenseFlow.Application.Services.Token;

public interface ITokenService
{
    string GenerateToken(UserModel user, IEnumerable<string> permissions);
}