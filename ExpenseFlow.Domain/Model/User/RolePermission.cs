using ExpenseFlow.Domain.Model.Base;

namespace ExpenseFlow.Domain.Model.User;

public class RolePermission : BaseModel
{
    public long RoleId { get; set; }
    public Role Role { get; set; }








    public long PermissionId { get; set; }
    public Permission Permission { get; set; }
}