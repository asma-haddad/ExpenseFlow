using ExpenseFlow.Domain.Model.Base;
using ExpenseFlow.Domain.Shared.Enum;

namespace ExpenseFlow.Domain.Model.User;

public class RoleModel : BaseModel
{
    public string Name { get; set; }
    public RoleType RoleType { get; set; }
    public ICollection<UserModel> Users { get; set; } = new List<UserModel>();

    public ICollection<PermissionRoleModel> RolePermissions { get; set; } = new List<PermissionRoleModel>();
}