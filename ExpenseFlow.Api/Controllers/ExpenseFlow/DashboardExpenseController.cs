using ExpenseFlow.Api.Authorization;
using ExpenseFlow.Application.Features.Expense.Command.Add;
using ExpenseFlow.Domain.Base;
using ExpenseFlow.Domain.Shared.Enum;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace ExpenseFlow.Api.Controllers.ExpenseFlow
{
    [ApiController]
    [Route("api/Dashboard/[controller]/[action]")]
    public class DashboardExpenseController(ISender sender) : ControllerBase
    {
        [HttpPost]
        //    [Produces(typeof(GetAllDataResponse<GetAllUserQuery.Response>))]
        [DashboardAuthorized((PermissionType.AddExpense))]
        public async Task<IActionResult> AddExpense([FromBody] AddExpenseCommand.Request request)
        {
            var result = await sender.Send(request);
            return result.GetResult();
        }
    }
}





