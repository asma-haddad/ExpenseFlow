using ExpenseFlow.Application.Abstraction;
using ExpenseFlow.Domain.Base.Dto;

namespace ExpenseFlow.Application.Features.Dashboard.Role.Query.GetAll
{
    public class GetAllRoleQuery
    {
        public class Request : SearchRequest, IQuery<GetAllDataResponse<Response>>
        {

        }
        public class Response
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string RoleType { get; set; }
        }
    }
}
