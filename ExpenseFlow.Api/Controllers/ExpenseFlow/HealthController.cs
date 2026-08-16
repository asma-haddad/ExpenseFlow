using ExpenseFlow.Api.Authorization;
using ExpenseFlow.Domain.Shared.Enum;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseFlow.Api.Controllers.ExpenseFlow;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    [DashboardAuthorized(PermissionType.ExpenseApprove)]
    public IActionResult Get()
    {
        return Ok(new { status = "ok", utc = DateTime.UtcNow });
    }
}