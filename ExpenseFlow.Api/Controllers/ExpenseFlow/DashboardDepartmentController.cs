using ExpenseFlow.Api.Authorization;
using ExpenseFlow.Application.Features.Dashboard.Department.Command.Add;
using ExpenseFlow.Application.Features.Dashboard.Department.Query;
using ExpenseFlow.Domain.Base;
using ExpenseFlow.Domain.Base.Dto;
using ExpenseFlow.Domain.Shared.Enum;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseFlow.Api.Controllers.ExpenseFlow
{
    [ApiController]
    [Route("api/Dashboard/[controller]/[action]")]
    [DashboardAuthorized((PermissionType.DepartmentManage))]

    public class DashboardDepartmentController(ISender sender) : ControllerBase
    {
        [HttpGet]
        [Produces(typeof(GetAllDataResponse<GetAllDepartmentQuery.Response>))]
        public async Task<IActionResult> GetAllDepartment([FromQuery] GetAllDepartmentQuery.Request request)
        {
            var result = await sender.Send(request);
            return result.GetResult();
        }
        [HttpPost]
        public async Task<IActionResult> AddDepartment([FromBody] AddDepartmentCommand.Request request)
        {
            var result = await sender.Send(request);
            return result.GetResult();
        }

    }
}

