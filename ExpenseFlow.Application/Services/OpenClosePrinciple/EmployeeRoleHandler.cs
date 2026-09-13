using ExpenseFlow.Application.Services.Interface;
using ExpenseFlow.Domain.Model.User;
using ExpenseFlow.Domain.Shared.Enum;

namespace ExpenseFlow.Application.Services.Cop
{
    public class EmployeeRoleHandler : IUserRoleHandler
    {
        public RoleType RoleType => RoleType.Employee;

        public bool RequiresDepartment => true;

        public Task BeforeSaveAsync(
            UserModel user,
            Guid? departmentId,
            CancellationToken cancellationToken)
        {
            if (departmentId == null)
            {
                throw new ArgumentNullException(
                    nameof(departmentId),
                    "Department is required for employee");
            }

            user.DepartmentId = departmentId.Value;

            return Task.CompletedTask;
        }

        public Task AfterSaveAsync(
            UserModel user,
            Guid? departmentId,
            CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}