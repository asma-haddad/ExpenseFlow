namespace ExpenseFlow.Application.Features.Dashboard.User.Command.AddBulk
{
    public class AddBulkUserCommand
    {
        public class Request : Abstraction.ICommand
        {
            public Guid DepartmentId { get; set; }
            public List<EmployeeDto> Employees { get; set; }
            public class EmployeeDto
            {
                public string FirstName { get; set; }
                public string LastName { get; set; }
                public string Email { get; set; }
                public string Password { get; set; }
            }
        }
    }
}
