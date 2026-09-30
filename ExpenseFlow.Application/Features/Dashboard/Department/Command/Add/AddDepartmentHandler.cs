using ExpenseFlow.Application.Abstraction;
using ExpenseFlow.Domain.Base;
using ExpenseFlow.Domain.Base.Language;
using ExpenseFlow.Domain.Model.Department;
using ExpenseFlow.Infrastructure.Data;
using Microsoft.AspNetCore.Http;

namespace ExpenseFlow.Application.Features.Dashboard.Department.Command.Add
{
    public class AddDepartmentHandler : BaseService, ICommandHandler<AddDepartmentCommand.Request>
    {
        public AddDepartmentHandler(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }
        public async Task<Result> Handle(AddDepartmentCommand.Request request, CancellationToken cancellationToken)
        {
            var result = new Result();


            var deptment = new DepartmentModel
            {
                ManagerId = request.ManagerId,
                Title = request.Title.ToModel(),
                Description = request.Description.ToModel(),

            };
            await context.Department.AddAsync(deptment, cancellationToken);
            await context.SaveChangesAsync();
            return result;
        }
    }
}
