using ExpenseFlow.Application.Abstraction;

namespace ExpenseFlow.Application.Features.Dashboard.User.Command.AddRange
{
    public class AddRangeUserCommand
    {
        public class Request : ICommand
        {
            public Guid DepartmentId { get; set; }

            public List<EmployeeDto> Employees { get; set; } = new();
        }

        public class EmployeeDto
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }
    }


}

