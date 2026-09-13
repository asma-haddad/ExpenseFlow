using ExpenseFlow.Domain.Model.User;
using ExpenseFlow.Domain.Shared.Enum;

namespace ExpenseFlow.Application.Services.Interface
{
    public interface IUserRoleHandler
    {
        RoleType RoleType { get; }

        bool RequiresDepartment { get; }

        Task BeforeSaveAsync(UserModel user, Guid? departmentId, CancellationToken cancellationToken);

        Task AfterSaveAsync(
            UserModel user,
            Guid? departmentId,
            CancellationToken cancellationToken);
    }
}
