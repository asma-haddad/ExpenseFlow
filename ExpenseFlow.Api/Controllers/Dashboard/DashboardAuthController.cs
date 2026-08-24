using ExpenseFlow.Application.Features.Dashboard.Auth.Login;
using ExpenseFlow.Domain.Base;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace ExpenseFlow.Api.Controllers.Dashboard
{
    [Route("api/Dashboard/[controller]/[action]")]
    [ApiController]
    public class DashboardAuthController(ISender sender) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Login(LoginCommand.Request request)
        {
            var result = await sender.Send(request);
            return result.GetResult();
        }
    }
}



