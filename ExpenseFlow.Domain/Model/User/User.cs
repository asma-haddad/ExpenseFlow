using ExpenseFlow.Domain.Model.Base;

namespace ExpenseFlow.Domain.Model.User;

public class User : BaseModel
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public long RoleId { get; set; }
    public Role Role { get; set; }
}