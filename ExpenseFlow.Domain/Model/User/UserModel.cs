using ExpenseFlow.Domain.Model.Base;
using ExpenseFlow.Domain.Model.Department;
using ExpenseFlow.Domain.Model.Expense;

namespace ExpenseFlow.Domain.Model.User;

public class UserModel : BaseModel
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public Guid RoleId { get; set; }
    public RoleModel Role { get; set; }

    public Guid? DepartmentId { get; set; }
    public DepartmentModel? Department { get; set; }
    public ICollection<ExpenseModel> Expenses { get; set; }
    public ICollection<DepartmentModel> ManagedDepartments { get; set; } = new List<DepartmentModel>();
}