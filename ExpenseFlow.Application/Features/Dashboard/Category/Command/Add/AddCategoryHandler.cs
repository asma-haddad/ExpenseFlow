using ExpenseFlow.Application.Abstraction;
using ExpenseFlow.Domain.Base;
using ExpenseFlow.Domain.Base.Language;
using ExpenseFlow.Domain.Model.Category;
using ExpenseFlow.Infrastructure.Data;
using Microsoft.AspNetCore.Http;

namespace ExpenseFlow.Application.Features.Dashboard.Category.Command.Add
{
    public class AddCategoryHandler : BaseService, ICommandHandler<AddCategoryCommand.Request>
    {
        public AddCategoryHandler(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }

        public async Task<Result> Handle(AddCategoryCommand.Request request, CancellationToken cancellationToken)
        {
            var result = new Result();

            var category = new CategoryModel
            {
                Title = request.Title.ToModel()
            };
            await context.Category.AddAsync(category, cancellationToken);
            await context.SaveChangesAsync();
            return result;
        }
    }
}
