using ExpenseFlow.Application.Abstraction;
using ExpenseFlow.Infrastructure.Data;
using Microsoft.AspNetCore.Http;

namespace ExpenseFlow.Application.Features.Dashboard.User.Command.Add
{
    public class AddUserHandler : BaseService, ICommandHandler<AddUserCommand.Request>
    {
        public AddUserHandler(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }

        public Task Handle(AddUserCommand.Request request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
