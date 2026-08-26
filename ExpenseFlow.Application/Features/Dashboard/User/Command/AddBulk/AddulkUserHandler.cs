using EFCore.BulkExtensions;
using ExpenseFlow.Application.Abstraction;
using ExpenseFlow.Domain.Base;
using ExpenseFlow.Domain.Model.User;
using ExpenseFlow.Domain.Shared.Enum;
using ExpenseFlow.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
namespace ExpenseFlow.Application.Features.Dashboard.User.Command.AddBulk
{
    public class AddulkUserHandler : BaseService, ICommandHandler<AddBulkUserCommand.Request>
    {
        public AddulkUserHandler(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }

        public async Task<Result> Handle(AddBulkUserCommand.Request request, CancellationToken cancellationToken)
        {
            var result = new Result();

            var department = await context.Department
                .FirstOrDefaultAsync(
                    x => x.Id == request.DepartmentId,
                    cancellationToken);

            if (department == null)
            {
                result.ThrowException(
                    ErrorMessages.NotFound,
                    ResultStatus.NotFound);
            }

            // 2. نجيب Role الموظف
            var employeeRole = await context.Role
                .FirstOrDefaultAsync(
                    x => x.RoleType == RoleType.Employee,
                    cancellationToken);

            if (employeeRole == null)
            {
                result.ThrowException(
                    ErrorMessages.NotFound,
                    ResultStatus.NotFound);
            }

            var employees = request.Employees
                .Select(x => new UserModel
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    PasswordHash = x.Password,

                    RoleId = employeeRole.Id,
                    DepartmentId = department.Id
                })
                .ToList();

            // 4. Bulk Insert
            await context.BulkInsertAsync(
              employees, cancellationToken: cancellationToken);

            return result;
        }
    }
}
