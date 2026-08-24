using ExpenseFlow.Application.Abstraction;
using ExpenseFlow.Domain.Base.Dto;
using ExpenseFlow.Domain.Base.Language;
using ExpenseFlow.Domain.Shared.Enum;

namespace ExpenseFlow.Application.Features.Dashboard.Expense.Query.GetAll
{
    public class GetAllExpenseQuery
    {
        public class Request : SearchRequest, IQuery<GetAllDataResponse<Response>>
        {

        }
        public class Response
        {
            public Guid Id { get; set; }
            public UserDto User { get; set; }
            public CategoryDto Category { get; set; }
            public double Amount { get; set; } = 0;
            public LanguagePropertyDto Title { get; set; }
            public string ReceiptImageUrl { get; set; } = null;
            public LanguagePropertyDto Description { get; set; }
            public ExpenseStatus ExpenseStatus { get; set; }

            public class CategoryDto
            {
                public Guid Id { get; set; }
                public LanguagePropertyDto Name { get; set; }
            }

            public class UserDto
            {
                public Guid UserId { get; set; }
                public string Name { get; set; }

            }
        }
    }

}






