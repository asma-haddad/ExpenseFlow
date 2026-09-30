
using ExpenseFlow.Application.Abstraction;
using ExpenseFlow.Domain.Base.Dto;
using ExpenseFlow.Domain.Base.Language;

namespace ExpenseFlow.Application.Features.Dashboard.Category.Query.GetAll
{
    public class GetAllCategoryQuery
    {
        public class Request : SearchRequest, IQuery<GetAllDataResponse<Response>>
        {

        }
        public class Response
        {
            public Guid Id { get; set; }
            public LanguagePropertyDto Tilte { get; set; }
        }

    }
}