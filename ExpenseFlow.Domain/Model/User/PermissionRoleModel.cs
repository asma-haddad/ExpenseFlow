using ExpenseFlow.Domain.Model.Base;

namespace ExpenseFlow.Domain.Model.User;

public class PermissionRoleModel : BaseModel
{
    public Guid RoleId { get; set; }
    public RoleModel Role { get; set; }

    public Guid PermissionId { get; set; }
    public PermissionModel Permission { get; set; }
}