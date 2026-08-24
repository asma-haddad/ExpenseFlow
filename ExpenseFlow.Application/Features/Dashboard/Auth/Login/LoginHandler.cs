using ExpenseFlow.Application.Abstraction;
using ExpenseFlow.Application.Services.Token;
using ExpenseFlow.Domain.Base;
using ExpenseFlow.Domain.Model.User;
using ExpenseFlow.Domain.Shared.Enum;
using ExpenseFlow.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ExpenseFlow.Application.Features.Dashboard.Auth.Login
{
    public class LoginHandler : BaseService, ICommandHandler<LoginCommand.Request, LoginCommand.Response>
    {
        private readonly ITokenService tokenService;
        public LoginHandler(AppDbContext context, IHttpContextAccessor httpContextAccessor, ITokenService tokenService) : base(context, httpContextAccessor)
        {
            this.tokenService = tokenService;
        }

        public async Task<Result<LoginCommand.Response>> Handle(LoginCommand.Request request, CancellationToken cancellationToken)
        {
            var result = new Result<LoginCommand.Response>();
            var user = await context.User.Include(r => r.Role).FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);
            if (user == null)
            {
                result.ThrowException(ErrorMessages.InvalidEmailOrPassword, ResultStatus.ValidationError);

            }
            //if (!user.Password.Equals(request.Password))
            //    result.ThrowException(ErrorMessages.InvalidEmailOrPassword, ResultStatus.ValidationError);

            var ip = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? request.Session?.IP;
            var ua = _httpContextAccessor.HttpContext?.Request?.Headers?.UserAgent.ToString() ?? request.Session?.BrowserInfo;

            var token = await tokenService.IssueTokensAsync(user.Id, ip, ua, false);
            var session = await context.Session.AddAsync(new SessionModel
            {
                UserId = user.Id,
                RefId = user.Id,
                AccessToken = token.Token,
                RefreshToken = token.RefreshToken,
                Language = acceptLanguage,

            });
            await context.SaveChangesAsync(cancellationToken);

            result.Data = new LoginCommand.Response
            {
                Token = token.Token,
            };

            return result;
        }
    }
}


