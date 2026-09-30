using ExpenseFlow.Application.Abstraction;
using ExpenseFlow.Application.Extensions;
using ExpenseFlow.Domain.Base;
using ExpenseFlow.Domain.Base.Dto;
using ExpenseFlow.Domain.Base.Language;
using ExpenseFlow.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ExpenseFlow.Application.Features.Dashboard.Category.Query.GetAll
{
    public class GetAllCategoryHandler : BaseService, IQueryHandler<GetAllCategoryQuery.Request, GetAllDataResponse<GetAllCategoryQuery.Response>>
    {
        public GetAllCategoryHandler(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }

        public async Task<Result<GetAllDataResponse<GetAllCategoryQuery.Response>>> Handle(GetAllCategoryQuery.Request request, CancellationToken cancellationToken)
        {
            var result = new Result<GetAllDataResponse<GetAllCategoryQuery.Response>>();

            result.Data = await context.Category.AsNoTracking()
           .Select(c => new GetAllCategoryQuery.Response
           {
               Id = c.Id,
               Tilte = c.Title.ToDto()

           }).PaginateAsync(request);

            return result;

        }
    }
}


