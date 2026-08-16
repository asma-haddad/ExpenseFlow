using ExpenseFlow.Application.Abstraction;
using ExpenseFlow.Domain.Base.Language;
using ExpenseFlow.Domain.Shared.Enum;
namespace ExpenseFlow.Application.Features.Dashboard.User.Command.Add
{
    public class AddUserCommand
    {
        public class Request : ICommand
        {
            public RoleType? RoleType { get; set; }
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

            public RoleDto Role { get; set; }

            public class RoleDto
            {
                public Guid Id { get; set; }
                public LanguagePropertyDto Name { get; set; }
            }
        }
    }


}
