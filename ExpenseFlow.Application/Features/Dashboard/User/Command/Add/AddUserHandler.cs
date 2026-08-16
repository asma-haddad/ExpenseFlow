//using ExpenseFlow.Application.Abstraction;
//using ExpenseFlow.Application.Extensions;
//using ExpenseFlow.Application.Features.Dashboard.User.Query.GetAll;
//using ExpenseFlow.Domain.Base;
//using ExpenseFlow.Domain.Base.Dto;
//using ExpenseFlow.Infrastructure.Data;
//using Microsoft.AspNetCore.Http;
//using Microsoft.EntityFrameworkCore;
//using System.Linq.Dynamic.Core;

//namespace ExpenseFlow.Application.Features.Dashboard.User.Command.Add
//{

//    public class AddUserHandler : BaseService, ICommandHandler<AddUserCommand.Request>
//    {
//        private readonly ParsingConfig _dynamicLinqConfig;


//        public AddUserHandler(AppDbContext context, IHttpContextAccessor httpContextAccessor, ParsingConfig dynamicLinqConfig) : base(context, httpContextAccessor)
//        {
//            _dynamicLinqConfig = dynamicLinqConfig;

//        }


//        public async Task Handle(AddUserCommand.Request request, CancellationToken cancellationToken)
//        {
//            var result = new Result<GetAllDataResponse<GetAllUserQuery.Response>>();
//            var query = context.User
//           .AsNoTracking()
//           .AsQueryable();

//            if (request.RoleType.HasValue)
//            {
//                query = query.Where(x => x.Role.RoleType == request.RoleType.Value);
//            }
//            result.Data = await query.OrderByDescending(x => x.CreatedAt)
//                .PaginateAsync(u => new GetAllUserQuery.Response
//                {
//                    Id = u.Id,
//                    Email = u.Email,
//                    LastName = u.LastName,
//                    FirstName = u.FirstName,


//                }, request, cancellationToken);

//            return result;
//        }
//    }
//}
