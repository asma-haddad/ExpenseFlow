using ExpenseFlow.Application.Abstraction;
using ExpenseFlow.Application.Extensions;
using ExpenseFlow.Domain.Base;
using ExpenseFlow.Domain.Base.Dto;
using ExpenseFlow.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ExpenseFlow.Application.Features.Dashboard.User.Query.GetAll
{
    public class GetAllUserHandler : BaseService, IQueryHandler<GetAllUserQuery.Request, GetAllDataResponse<GetAllUserQuery.Response>>
    {
        public GetAllUserHandler(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }


        public async Task<Result<GetAllDataResponse<GetAllUserQuery.Response>>> Handle(
    GetAllUserQuery.Request request,
    CancellationToken cancellationToken)
        {
            var result =
                new Result<GetAllDataResponse<GetAllUserQuery.Response>>();

            var query = context.User
                .AsNoTracking()
                .AsQueryable();

            result.Data = await query
                .OrderByDescending(x => x.CreatedAt)
                .PaginateAsync(
                    u => new GetAllUserQuery.Response
                    {
                        Id = u.Id,
                        Email = u.Email,
                        FirstName = u.FirstName,
                        LastName = u.LastName
                    },
                    request,
                    cancellationToken);

            return result;
        }
    }
}
