using ExpenseFlow.Application.Abstraction;
using ExpenseFlow.Domain.Base.Dto;
using ExpenseFlow.Domain.Base.Language;
namespace ExpenseFlow.Application.Features.Dashboard.User.Query.GetAll
{
    public class GetAllUserQuery
    {

        public class Request : SearchRequest, IQuery<GetAllDataResponse<Response>>
        {
        }

        public class Response
        {
            public Guid Id { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
            public string Address { get; set; }
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public bool ViewOnlyAssignedLeads { get; set; }
            public LanguagePropertyDto City { get; set; }
            public RoleDto Role { get; set; }

            public class RoleDto
            {
                public Guid Id { get; set; }
            }
        }
    }


}
