using ExpenseFlow.Domain.Model.Base;

namespace ExpenseFlow.Domain.Model.User;

public class UserModel : BaseModel
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public Guid RoleId { get; set; }
    public RoleModel Role { get; set; }

    public Guid? ManagerId { get; set; }
    public UserModel Manager { get; set; }

    public ICollection<UserModel> Employees { get; set; } = new List<UserModel>();
}