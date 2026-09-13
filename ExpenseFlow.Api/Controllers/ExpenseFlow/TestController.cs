using Microsoft.AspNetCore.Mvc;

namespace ExpenseFlow.Api.Controllers.ExpenseFlow;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok(new { message = "ExpenseFlow API is running" });
    }
}
