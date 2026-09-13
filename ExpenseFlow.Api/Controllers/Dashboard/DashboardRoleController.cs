using ExpenseFlow.Api.Authorization;
using ExpenseFlow.Application.Features.Dashboard.Role.Query.GetAll;
using ExpenseFlow.Domain.Base;
using ExpenseFlow.Domain.Base.Dto;
using ExpenseFlow.Domain.Shared.Enum;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseFlow.Api.Controllers.Dashboard
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardRoleController(ISender sender) : ControllerBase
    {
        [HttpGet]
        [DashboardAuthorized(PermissionType.GetRole)]
        [Produces(typeof(GetAllDataResponse<GetAllRoleQuery.Response>))]
        public async Task<IActionResult> GetAllExpense([FromQuery] GetAllRoleQuery.Request request)
        {
            var result = await sender.Send(request);
            return result.GetResult();
        }
    }
}
