using ExpenseFlow.Application.Abstraction;
using ExpenseFlow.Domain.Base;
using ExpenseFlow.Domain.Model.User;
using ExpenseFlow.Domain.Shared.Enum;
using ExpenseFlow.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ExpenseFlow.Application.Features.Dashboard.User.Command.AddRange
{
    public class AddRangeUserHandler : BaseService, ICommandHandler<AddRangeUserCommand.Request>
    {
        public AddRangeUserHandler(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }

        public async Task<Result> Handle(AddRangeUserCommand.Request request, CancellationToken cancellationToken)
        {
            var result = new Result();
            var department = await context.Department.FirstOrDefaultAsync(x => x.Id == request.DepartmentId, cancellationToken);

            if (department == null)
            {
                result.ThrowException(ErrorMessages.NotFound, ResultStatus.NotFound);
            }
            var employeeRole = await context.Role
                .FirstOrDefaultAsync(
                    x => x.RoleType == RoleType.Employee,
                    cancellationToken);

            if (employeeRole == null)
            {
                result.ThrowException(ErrorMessages.NotFound, ResultStatus.NotFound);
            }
            var users = request.Employees
                .Select(employee => new UserModel
                {
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Email = employee.Email,
                    PasswordHash = employee.Password,
                    RoleId = employeeRole.Id,

                    DepartmentId = department.Id
                })
                .ToList();

            await context.User.AddRangeAsync(users, cancellationToken);

            await context.SaveChangesAsync(cancellationToken);

            return result;
        }
    }
}

