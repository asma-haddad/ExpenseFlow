using ExpenseFlow.Api.Authorization;
using ExpenseFlow.Application.Features.Dashboard.User.Command.Add;
using ExpenseFlow.Application.Features.Dashboard.User.Command.AddBulk;
using ExpenseFlow.Application.Features.Dashboard.User.Command.AddRange;
using ExpenseFlow.Application.Features.Dashboard.User.Query.GetAll;
using ExpenseFlow.Domain.Base;
using ExpenseFlow.Domain.Base.Dto;
using ExpenseFlow.Domain.Shared.Enum;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace ExpenseFlow.Api.Controllers.Dashboard
{
    [Route("api/Dashboard/[controller]/[action]")]
    [ApiController]
    [DashboardAuthorized(PermissionType.AddUser)]
    public class DashboardUserController(ISender sender) : ControllerBase
    {
        [HttpGet]
        [Produces(typeof(GetAllDataResponse<GetAllUserQuery.Response>))]
        public async Task<IActionResult> GetAllExpense([FromQuery] GetAllUserQuery.Request request)
        {
            var result = await sender.Send(request);
            return result.GetResult();
        }
        [HttpPost]
        public async Task<IActionResult> AddUser(AddUserCommand.Request request)
        {
            var result = await sender.Send(request);
            return result.GetResult();
        }
        [HttpPost]
        public async Task<IActionResult> AddRangeUser(AddRangeUserCommand.Request request)
        {
            var result = await sender.Send(request);
            return result.GetResult();
        }
        [HttpPost]
        public async Task<IActionResult> AddBulkUser(AddBulkUserCommand.Request request)
        {
            var result = await sender.Send(request);
            return result.GetResult();
        }
    }
}
