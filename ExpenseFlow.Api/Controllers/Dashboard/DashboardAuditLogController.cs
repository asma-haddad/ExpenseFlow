using ExpenseFlow.Application.Features.Dashboard.AuditLog.GetAllAuditLog;
using ExpenseFlow.Domain.Base;
using ExpenseFlow.Domain.Base.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace ExpenseFlow.Api.Controllers.Dashboard
{
    [Route("api/Dashboard/[controller]/[action]")]
    [ApiController]
    public class DashboardAuditLogController(ISender sender) : ControllerBase
    {
        [HttpGet]
        [Produces(typeof(GetAllDataResponse<GetAllAuditLogQuery.Response>))]

        public async Task<IActionResult> GetAllAuditLog([FromQuery] GetAllAuditLogQuery.Request request)
        {
            var result = await sender.Send(request);
            return result.GetResult();
        }
    }
}
