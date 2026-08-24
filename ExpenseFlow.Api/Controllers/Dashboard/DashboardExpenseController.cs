using ExpenseFlow.Api.Authorization;
using ExpenseFlow.Application.Features.Dashboard.Expense.Query.GetAll;
using ExpenseFlow.Domain.Base;
using ExpenseFlow.Domain.Base.Dto;
using ExpenseFlow.Domain.Shared.Enum;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace ExpenseFlow.Api.Controllers.Dashboard
{
    [Route("api/Dashboard/[controller]/[action]")]
    [ApiController]
    public class DashboardExpenseController(ISender sender) : ControllerBase
    {
        [HttpGet]
        [DashboardAuthorized(
        PermissionType.ExpenseViewOwn,
        PermissionType.ExpenseViewDepartment,
        PermissionType.ExpenseViewAll)]
        [Produces(typeof(GetAllDataResponse<GetAllExpenseQuery.Response>))]
        public async Task<IActionResult> GetAllExpense([FromQuery] GetAllExpenseQuery.Request request)
        {
            var result = await sender.Send(request);
            return result.GetResult();
        }
    }
}

//public class DashbordCategoryController(ISender sender) : ControllerBase
//{
//    [HttpGet]
//    [DashboardAuthorized(nameof(PermissionType.GetCategory))]
//    [Produces(typeof(GetAllDataResponse<GetAllCategoryQuery.Response>))]
//    public async Task<IActionResult> GetAllCategory([FromQuery] GetAllCategoryQuery.Request request)
//    {
//        var result = await sender.Send(request);
//        return result.GetResult();
//    }
//    [HttpPost]
//    [DashboardAuthorized(nameof(PermissionType.AddCategory))]
//    public async Task<IActionResult> AddCategory(AddCategoryCommand.Request request)
//    {
//        var result = await sender.Send(request);
//        return result.GetResult();
//    }