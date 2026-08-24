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

            // 1- تأكد من الـ Role
            var role = await context.Role
                .FirstOrDefaultAsync(
                    x => x.Id == request.RoleId,
                    cancellationToken);

            if (role == null)
            {
                result.ThrowException(
                    ErrorMessages.NotFound,
                    ResultStatus.NotFound);
            }

            var needsDepartment =
                role.RoleType == RoleType.Employee ||
                role.RoleType == RoleType.Manager;

            if (needsDepartment && request.DepartmentId == null)
            {
                result.ThrowException(
                    "Department is required",
                    ResultStatus.ValidationError);
            }

            // 3- جيبي القسم وتحققي منه قبل حفظ المستخدم
            var department = needsDepartment
                ? await context.Department
                    .FirstOrDefaultAsync(
                        x => x.Id == request.DepartmentId,
                        cancellationToken)
                : null;

            if (needsDepartment && department == null)
            {
                result.ThrowException(
                    ErrorMessages.NotFound,
                    ResultStatus.NotFound);
            }

            // 4- بعد ما تأكدنا من كلشي، أنشئ المستخدم
            var user = new UserModel
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = request.Password,
                RoleId = request.RoleId,
            };

            // 5- إذا Employee اربطيه بالقسم
            if (role.RoleType == RoleType.Employee)
            {
                user.DepartmentId = department!.Id;
            }

            // 6- احفظي المستخدم
            await context.User.AddAsync(user, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            // 7- إذا Manager عيّنيه مدير للقسم
            if (role.RoleType == RoleType.Manager)
            {
                department!.ManagerId = user.Id;

                await context.SaveChangesAsync(cancellationToken);
            }

            return result;
        }
    }
}