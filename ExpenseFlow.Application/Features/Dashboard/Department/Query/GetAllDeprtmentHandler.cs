using ExpenseFlow.Application.Abstraction;
using ExpenseFlow.Application.Extensions;
using ExpenseFlow.Application.Services.Helper;
using ExpenseFlow.Domain.Base;
using ExpenseFlow.Domain.Base.Dto;
using ExpenseFlow.Domain.Base.Language;
using ExpenseFlow.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace ExpenseFlow.Application.Features.Dashboard.Department.Query
{
    public class GetAllDeprtmentHandler : BaseService, IQueryHandler<GetAllDepartmentQuery.Request, GetAllDataResponse<GetAllDepartmentQuery.Response>>
    {
        private readonly ParsingConfig _dynamicLinqConfig;
        public GetAllDeprtmentHandler(AppDbContext context, IHttpContextAccessor httpContextAccessor, ParsingConfig dynamicLinqConfig) : base(context, httpContextAccessor)
        {
            _dynamicLinqConfig = dynamicLinqConfig;
        }

        public async Task<Result<GetAllDataResponse<GetAllDepartmentQuery.Response>>> Handle(GetAllDepartmentQuery.Request request, CancellationToken cancellationToken)
        {
            var result = new Result<GetAllDataResponse<GetAllDepartmentQuery.Response>>();
            var query = context.Department.AsNoTracking();
            if (request.Filters != null)
            {
                query = QueryFilterHelper.ApplyFilters(query, request.Filters, request.IsAnd, acceptLanguage, _dynamicLinqConfig);
            }
            result.Data = await query
               .Select(c => new GetAllDepartmentQuery.Response
               {
                   Title = c.Title.ToDto(),
                   Description = c.Description.ToDto(),
                   Id = c.Id,
               }).SortBy(request.SortProperty, request.IsAsc, acceptLanguage)
               .PaginateAsync(request, cancellationToken);
            return result;
        }
    }
}
