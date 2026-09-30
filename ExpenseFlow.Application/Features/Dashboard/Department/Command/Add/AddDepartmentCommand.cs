using ExpenseFlow.Application.Abstraction;
using ExpenseFlow.Domain.Base.Language;

namespace ExpenseFlow.Application.Features.Dashboard.Department.Command.Add
{
    public class AddDepartmentCommand
    {
        public class Request : ICommand
        {
            public Guid ManagerId { get; set; }
            public LanguagePropertyDto Title { get; set; }
            public LanguagePropertyDto? Description { get; set; }
        }
    }
}
