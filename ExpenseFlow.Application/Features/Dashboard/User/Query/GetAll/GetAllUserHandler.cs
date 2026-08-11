using ExpenseFlow.Application.Abstraction;
using ExpenseFlow.Application.Extensions;
using ExpenseFlow.Domain.Base;
using ExpenseFlow.Domain.Base.Dto;
using ExpenseFlow.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using System.Linq.Dynamic.Core;
namespace ExpenseFlow.Application.Features.Dashboard.User.Query.GetAll
{
    public class GetAllUserHandler : BaseService, IQueryHandler<GetAllUserQuery.Request, GetAllDataResponse<GetAllUserQuery.Response>>
    {
        private readonly ParsingConfig _dynamicLinqConfig;

        public GetAllUserHandler(AppDbContext context, IHttpContextAccessor httpContextAccessor, ParsingConfig dynamicLinqConfig) : base(context, httpContextAccessor)
        {
            _dynamicLinqConfig = dynamicLinqConfig;
        }
        public async Task<Result<GetAllDataResponse<GetAllUserQuery.Response>>> Handle(GetAllUserQuery.Request request, CancellationToken cancellationToken)
        {
            var result = new Result<GetAllDataResponse<GetAllUserQuery.Response>>();

            var query = context.User/*.Where(p => request.Query == null || p.City.Search(request.Query))*/;


            if (request.Filters != null && request.Filters.Any())
            {
                // query = QueryFilterHelper.ApplyFilters(query, request.Filters, request.IsAnd, acceptLanguage, _dynamicLinqConfig);
            }
            //result.Data = await query
            //    .PaginateAsync(
            //        u => new GetAllUserQuery.Response
            //        {
            //            Id = u.Id,
            //            Email = u.Email,
            //            FirstName = u.FirstName,
            //            LastName = u.LastName
            //        }, request
            //        );


            result.Data = await query
                .PaginateAsync(
                    u => new GetAllUserQuery.Response
                    {
                        Id = u.Id,
                        Email = u.Email,

                        LastName = u.LastName,
                        FirstName = u.FirstName,
                        Role = new GetAllUserQuery.Response.RoleDto
                        {
                            Id = u.RoleId
                        },
                    }, request);

            return result;
        }
    }
}
