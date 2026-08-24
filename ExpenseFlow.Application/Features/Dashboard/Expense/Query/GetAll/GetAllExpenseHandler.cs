using ExpenseFlow.Application.Abstraction;
using ExpenseFlow.Application.Extensions;
using ExpenseFlow.Domain.Base;
using ExpenseFlow.Domain.Base.Dto;
using ExpenseFlow.Domain.Base.Language;
using ExpenseFlow.Domain.Shared.Enum;
using ExpenseFlow.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore.Extensions;
using System.Linq.Dynamic.Core;
using static ExpenseFlow.Application.Features.Dashboard.Expense.Query.GetAll.GetAllExpenseQuery;

namespace ExpenseFlow.Application.Features.Dashboard.Expense.Query.GetAll
{
    public class GetAllExpenseHandler : BaseService, IQueryHandler<GetAllExpenseQuery.Request, GetAllDataResponse<GetAllExpenseQuery.Response>>
    {
        private readonly ParsingConfig _dynamicLinqConfig;
        public GetAllExpenseHandler(AppDbContext context, IHttpContextAccessor httpContextAccessor, ParsingConfig dynamicLinqConfig) : base(context, httpContextAccessor)
        {
            _dynamicLinqConfig = dynamicLinqConfig;
        }
        public async Task<Result<GetAllDataResponse<GetAllExpenseQuery.Response>>> Handle(GetAllExpenseQuery.Request request, CancellationToken cancellationToken)
        {
            var result = new Result<GetAllDataResponse<GetAllExpenseQuery.Response>>();
            var currentUser = await context.User.Where(x => x.Id == UserId)
                .Select(x => new
                {
                    Permissions = x.Role.RolePermissions
                    .Select(x => x.Permission.Name)
                    .ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);
            var query = context.Expense
               .Where(
               x =>
                   request.Query == null
                   ||

                   EF.Functions.Like(x.User.FirstName + " " + x.User.LastName, $"%{request.Query}%")
                   ||
                   x.Title.Search(request.Query)
                   ||
                   x.Description.Search(request.Query)
                  )


               .AsNoTracking();
            if (currentUser == null)
            {
                result.ThrowException(ErrorMessages.UnAuthenticated, ResultStatus.UnAuthenticated);
            }

            else if (currentUser.Permissions.Contains(PermissionType.ExpenseViewAll.ToString()))
            {

            }
            else if (currentUser.Permissions.Contains(PermissionType.ExpenseViewDepartment.ToString()))
            {
                query = query.Where(x =>
             x.User.Department.ManagerId == UserId);

            }
            else if (currentUser.Permissions.Contains(PermissionType.ExpenseViewOwn.ToString()))
            {
                query = query.Where(x => x.UserId == UserId);
            }
            else
            {
                result.ThrowException(ErrorMessages.NotFound, ResultStatus.NotFound);
            }
            query = query.SortBy(request.SortProperty, request.IsAsc, acceptLanguage);
            var data = await query
                .PaginateAsync(
                    u => new Response
                    {
                        Id = u.Id,
                        Description = u.Description.ToDto(),
                        Title = u.Title.ToDto(),
                        Amount = u.Amount,
                        ExpenseStatus = u.ExpenseStatus,
                        ReceiptImageUrl = u.ReceiptImageUrl,
                        Category = new Response.CategoryDto
                        {
                            Id = u.CategoryId,
                            Name = u.Category.Title.ToDto(),
                        },
                        User = new Response.UserDto
                        {
                            UserId = u.UserId,
                            Name = u.User.FirstName + " " + u.User.LastName,
                        }
                    }, request, cancellationToken
                    );
            result.Data = data;

            return result;

        }
    }
}
