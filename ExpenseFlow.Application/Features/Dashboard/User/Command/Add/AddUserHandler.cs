using ExpenseFlow.Application.Abstraction;
using ExpenseFlow.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using System.Linq.Dynamic.Core;

namespace ExpenseFlow.Application.Features.Dashboard.User.Command.Add
{

    public class AddUserHandler : BaseService, ICommandHandler<AddUserCommand.Request>
    {
        private readonly ParsingConfig _dynamicLinqConfig;


        public AddUserHandler(AppDbContext context, IHttpContextAccessor httpContextAccessor, ParsingConfig dynamicLinqConfig) : base(context, httpContextAccessor)
        {
            _dynamicLinqConfig = dynamicLinqConfig;

        }


        public Task Handle(AddUserCommand.Request request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
