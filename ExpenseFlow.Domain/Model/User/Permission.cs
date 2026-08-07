using ExpenseFlow.Domain.Model.Base;

namespace ExpenseFlow.Domain.Model.User;

public class Permission : BaseModel
{
    public string Name { get; set; } = string.Empty;
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}