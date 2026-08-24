using ExpenseFlow.Application.Abstraction;
namespace ExpenseFlow.Application.Features.Dashboard.User.Command.Add
{
    public class AddUserCommand
    {
        public class Request : ICommand
        {

            public string Phone { get; set; }
            public string Email { get; set; }
            public string Address { get; set; }
            public string Password { get; set; }
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public Guid? DepartmentId { get; set; }
            public Guid RoleId { get; set; }

        }

        public class Response
        {
            public string Email { set; get; }
            public string Password { set; get; }
        }
    }

}
