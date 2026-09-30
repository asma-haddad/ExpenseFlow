using ExpenseFlow.Application.Abstraction;
using ExpenseFlow.Domain.Base.Dto;
using ExpenseFlow.Domain.Base.Language;

namespace ExpenseFlow.Application.Features.Dashboard.Department.Query
{
    public class GetAllDepartmentQuery
    {
        public class Request : SearchRequest, IQuery<GetAllDataResponse<Response>>
        {
        }
        public class Response
        {
            public Guid Id { set; get; }
            public LanguagePropertyDto Title { get; set; }
            public LanguagePropertyDto Description { set; get; }

        }
    }
}


