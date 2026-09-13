using ExpenseFlow.Application.Abstraction;
using ExpenseFlow.Application.Extensions;
using ExpenseFlow.Domain.Base;
using ExpenseFlow.Domain.Base.Dto;
using ExpenseFlow.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ExpenseFlow.Application.Features.Dashboard.Role.Query.GetAll
{
    internal class GetAllRoleHandler : BaseService, IQueryHandler<GetAllRoleQuery.Request, GetAllDataResponse<GetAllRoleQuery.Response>>
    {
        public GetAllRoleHandler(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }

        public async Task<Result<GetAllDataResponse<GetAllRoleQuery.Response>>> Handle(GetAllRoleQuery.Request request, CancellationToken cancellationToken)
        {
            var result = new Result<GetAllDataResponse<GetAllRoleQuery.Response>>();

            result.Data = await context.Role.AsNoTracking()
                .Select(x => new GetAllRoleQuery.Response
                {
                    Id = x.Id,
                    Name = x.Name,
                    RoleType = x.RoleType.ToString()
                }).PaginateAsync(request);

            return result;
        }
    }
}
