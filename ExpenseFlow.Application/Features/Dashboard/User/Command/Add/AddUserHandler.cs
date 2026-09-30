using ExpenseFlow.Application.Abstraction;
using ExpenseFlow.Application.Services.Helper;
using ExpenseFlow.Application.Services.Interface;
using ExpenseFlow.Domain.Base;
using ExpenseFlow.Domain.Model.User;
using ExpenseFlow.Domain.Shared.Enum;
using ExpenseFlow.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ExpenseFlow.Application.Features.Dashboard.User.Command.Add
{
    public class AddUserHandler : BaseService, ICommandHandler<AddUserCommand.Request>
    {
        private readonly IEnumerable<IUserRoleHandler> _roleHandlers;
        public AddUserHandler(AppDbContext context, IHttpContextAccessor httpContextAccessor, IEnumerable<IUserRoleHandler> roleHandlers) : base(context, httpContextAccessor)
        {
            _roleHandlers = roleHandlers;
        }
        public async Task<Result> Handle(AddUserCommand.Request request, CancellationToken cancellationToken)
        {
            var result = new Result();
            var role = await context.Role.FirstOrDefaultAsync(x => x.Id == request.RoleId, cancellationToken);
            if (role == null)
            {
                result.ThrowException(ErrorMessages.NotFound, ResultStatus.NotFound);
            }
            var roleHandler = _roleHandlers.FirstOrDefault(x => x.RoleType == role.RoleType);
            var needsDepartment = roleHandler?.RequiresDepartment ?? false;

            if (needsDepartment && request.DepartmentId == null)
            {
                result.ThrowException(ErrorMessages.DepartmentIsRequired, ResultStatus.ValidationError);
            }
            if (request.DepartmentId.HasValue)
            {
                var departmentId = request.DepartmentId.Value;
                var departmentExists = await context.Department
                    .AnyAsync(x => x.Id == departmentId, cancellationToken);
                if (!departmentExists)
                {
                    result.ThrowException(ErrorMessages.NotFound, ResultStatus.NotFound);
                }
            }
            var user = new UserModel
            {
                Email = request.Email,
                RoleId = request.RoleId,
                LastName = request.LastName,
                FirstName = request.FirstName,
                PasswordHash = PasswordHelper.HashPassword(request.Password),
                DepartmentId = request.DepartmentId
            };

            if (roleHandler != null)
            {
                await roleHandler.BeforeSaveAsync(user, request.DepartmentId, cancellationToken);
            }
            await context.User.AddAsync(user, cancellationToken);

            await context.SaveChangesAsync(cancellationToken);

            if (roleHandler != null)
            {
                await roleHandler.AfterSaveAsync(user, request.DepartmentId, cancellationToken);
            }
            return result;
        }
    }
}