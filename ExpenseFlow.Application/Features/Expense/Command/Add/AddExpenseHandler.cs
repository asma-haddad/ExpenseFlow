using ExpenseFlow.Application.Abstraction;
using ExpenseFlow.Domain.Base;
using ExpenseFlow.Domain.Base.Language;
using ExpenseFlow.Domain.Model.Expense;
using ExpenseFlow.Infrastructure.Data;
using Microsoft.AspNetCore.Http;

namespace ExpenseFlow.Application.Features.Expense.Command.Add
{
    public class AddExpenseHandler : BaseService, ICommandHandler<AddExpenseCommand.Request>
    {
        public AddExpenseHandler(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }
        public async Task<Result> Handle(AddExpenseCommand.Request request, CancellationToken cancellationToken)
        {
            var result = new Result();

            var entity = new ExpenseModel
            {
                Amount = request.Amount,
                ReceiptImageUrl = request.ReceiptImageUrl,
                Title = request.Title.ToModel(),
                Description = request.Description.ToModel(),
                CategoryId = request.CategoryId,
                UserId = request.UserId,
            };
            await context.Expense.AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync();
            return result;
        }
    }
}



