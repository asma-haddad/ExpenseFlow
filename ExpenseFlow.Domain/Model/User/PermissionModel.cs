using ExpenseFlow.Domain.Model.Base;
using ExpenseFlow.Domain.Shared.Enum;

namespace ExpenseFlow.Domain.Model.User;

public class PermissionModel : BaseModel
{
    public PermissionType Code { get; set; }

    public string Name { get; set; }

    public ICollection<PermissionRoleModel> RolePermissions { get; set; } = new List<PermissionRoleModel>();
}