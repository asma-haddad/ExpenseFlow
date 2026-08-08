using ExpenseFlow.Domain.Model.Base;

namespace ExpenseFlow.Domain.Model.User;

public class Role : BaseModel
{
    public string Name { get; set; } = string.Empty;
    public ICollection<UserModel> Users { get; set; } = new List<UserModel>();

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}