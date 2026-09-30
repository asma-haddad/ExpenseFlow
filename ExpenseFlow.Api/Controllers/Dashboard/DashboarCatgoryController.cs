using ExpenseFlow.Application.Features.Dashboard.Category.Command.Add;
using ExpenseFlow.Application.Features.Dashboard.Category.Query.GetAll;
using ExpenseFlow.Domain.Base;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace ExpenseFlow.Api.Controllers.Dashboard
{
    [Route("api/Dashboard/[controller]/[action]")]
    [ApiController]
    public class DashboarCatgoryController(ISender sender) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllCategory([FromQuery] GetAllCategoryQuery.Request request)
        {
            var result = await sender.Send(request);
            return result.GetResult();
        }
        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] AddCategoryCommand.Request request)
        {
            var result = await sender.Send(request);
            return result.GetResult();
        }
    }
}

