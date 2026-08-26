using ExpenseFlow.Application.Abstraction;
using ExpenseFlow.Domain.Base;
using ExpenseFlow.Domain.Model.User;
using ExpenseFlow.Domain.Shared.Enum;
using ExpenseFlow.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace ExpenseFlow.Application.Features.Dashboard.User.Command.Add
{

    public class AddUserHandler : BaseService, ICommandHandler<AddUserCommand.Request>
    {
        private readonly ParsingConfig _dynamicLinqConfig;
        public AddUserHandler(AppDbContext context, IHttpContextAccessor httpContextAccessor, ParsingConfig dynamicLinqConfig) : base(context, httpContextAccessor)
        {
            _dynamicLinqConfig = dynamicLinqConfig;
        }
        public async Task<Result> Handle(AddUserCommand.Request request, CancellationToken cancellationToken)
        {
            var result = new Result();

            var role = await context.Role.FirstOrDefaultAsync(x => x.Id == request.RoleId, cancellationToken);

            if (role == null)
            {
                result.ThrowException(ErrorMessages.NotFound, ResultStatus.NotFound);
            }

            var needsDepartment =
                role.RoleType == RoleType.Employee || role.RoleType == RoleType.Manager;

            if (needsDepartment && request.DepartmentId == null)
            {
                result.ThrowException("Department is required", ResultStatus.ValidationError);
            }

            var department = needsDepartment ? await context.Department.FirstOrDefaultAsync(x => x.Id == request.DepartmentId, cancellationToken) : null;

            if (needsDepartment && department == null)
            {
                result.ThrowException(ErrorMessages.NotFound, ResultStatus.NotFound);
            }

            var user = new UserModel
            {
                Email = request.Email,
                RoleId = request.RoleId,
                LastName = request.LastName,
                FirstName = request.FirstName,
                PasswordHash = request.Password,
            };
            if (role.RoleType == RoleType.Employee)
            {
                user.DepartmentId = department!.Id;
            }
            await context.User.AddAsync(user, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            if (role.RoleType == RoleType.Manager)
            {
                department!.ManagerId = user.Id;

                await context.SaveChangesAsync(cancellationToken);
            }
            return result;
        }
    }
}