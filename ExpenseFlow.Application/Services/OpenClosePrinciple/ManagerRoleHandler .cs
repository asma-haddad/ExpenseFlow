using ExpenseFlow.Application.Services.Interface;
using ExpenseFlow.Domain.Model.User;
using ExpenseFlow.Domain.Shared.Enum;
using ExpenseFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseFlow.Application.Services.Cop
{
    public class ManagerRoleHandler : IUserRoleHandler
    {
        private readonly AppDbContext _context;

        public ManagerRoleHandler(AppDbContext context)
        {
            _context = context;
        }

        public RoleType RoleType => RoleType.Manager;

        public bool RequiresDepartment => true;


        public Task BeforeSaveAsync(
            UserModel user,
            Guid? departmentId,
            CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }


        // After saving user, user.Id is available
        public async Task AfterSaveAsync(
            UserModel user,
            Guid? departmentId,
            CancellationToken cancellationToken)
        {
            if (departmentId == null)
                return;

            var department = await _context.Department
                .FirstOrDefaultAsync(
                    x => x.Id == departmentId.Value,
                    cancellationToken);

            if (department == null)
                return;

            department.ManagerId = user.Id;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}