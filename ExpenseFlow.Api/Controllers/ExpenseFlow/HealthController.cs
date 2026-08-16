using ExpenseFlow.Api.Authorization;
using ExpenseFlow.Application.Features.Dashboard.User.Query.GetAll;
using ExpenseFlow.Domain.Base;
using ExpenseFlow.Domain.Base.Dto;
using ExpenseFlow.Domain.Shared.Enum;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace ExpenseFlow.Api.Controllers.ExpenseFlow;

[ApiController]
[Route("api/[controller]")]
public class HealthController(ISender sender) : ControllerBase
{
    [HttpGet]
    [Produces(typeof(GetAllDataResponse<GetAllUserQuery.Response>))]
    [DashboardAuthorized((PermissionType.ExpenseApprove))]
    public async Task<IActionResult> GetAllUser([FromQuery] GetAllUserQuery.Request request)
    {
        var result = await sender.Send(request);
        return result.GetResult();
    }
}