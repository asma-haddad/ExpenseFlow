//using ExpenseFlow.Domain.Shared.Enum;
//using MediatR;
//using Microsoft.AspNetCore.Mvc;
//using static ExpenseFlow.Application.Features.Dashboard.User.Command.Add.AddUserCommand;

//namespace ExpenseFlow.Api.Controllers
//{
//    [Route("api/Dashboard/[controller]/[action]")]
//    [ApiController]
//    public class DashbordCategoryController(ISender sender) : ControllerBase
//    {
//        [HttpGet]
//        [Produces(typeof(GetAllDataResponse<AddUserCommand.Request>))]
//        [DashboardAuthorized(nameof(PermissionType.GetCategory))]
//        public async Task<IActionResult> GetAllCity([FromQuery] .Request request)
//        {
//            var result = await sender.Send(request);
//            return result.GetResult();
//        }
//    }
