
using ExpenseFlow.Application.Abstraction;
using ExpenseFlow.Domain.Base.Language;
namespace ExpenseFlow.Application.Features.Dashboard.Category.Command.Add
{
    public class AddCategoryCommand
    {
        public class Request : ICommand
        {
            public LanguagePropertyDto Title { get; set; }

        }

    }
}
