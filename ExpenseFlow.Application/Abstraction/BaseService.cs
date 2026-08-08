using ExpenseFlow.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ExpenseFlow.Application.Abstraction
{
    public class BaseService
    {
        protected string acceptLanguage => GetAcceptedLanguage();
        protected Guid UserId => GetUserId();

        protected AppDbContext context;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public BaseService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetAcceptedLanguage()
        {
            var currentLanguage = "";
            if (_httpContextAccessor.HttpContext != null)
            {
                currentLanguage = _httpContextAccessor.HttpContext.Request
                    .Headers["accept-language"];

                /* allAcceptLanguage = _httpContextAccessor.HttpContext.Request
                     .Headers["accept-language"];*/
            }

            if (string.IsNullOrEmpty(currentLanguage) ||
                currentLanguage.StartsWith("en"))
            {
                return "en";
            }

            if (currentLanguage.StartsWith("ar"))
            {
                return "ar";
            }

            return "en";
        }

        private Guid GetUserId()
        {
            var id = Guid.Empty;
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext?.User?.Identity is { IsAuthenticated: true })
                {
                    id = Guid.Parse(httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
                }
            }
            catch (Exception)
            {
                id = Guid.Empty;
            }

            return id;
        }
    }

}

