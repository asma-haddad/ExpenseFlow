using ExpenseFlow.Application.Abstraction;
using ExpenseFlow.Domain.Base.Language;
namespace ExpenseFlow.Application.Features.Expense.Command.Add
{
    public class AddExpenseCommand
    {
        public class Request : ICommand
        {
            public Guid UserId { get; set; }
            public Guid CategoryId { get; set; }

            public double Amount { get; set; } = 0;
            public string ReceiptImageUrl { get; set; }
            public LanguagePropertyDto Title { get; set; }
            public LanguagePropertyDto Description { get; set; }

        }
    }

}

