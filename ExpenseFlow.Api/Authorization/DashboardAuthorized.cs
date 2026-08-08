using ExpenseFlow.Domain.Base;
using ExpenseFlow.Domain.Shared.Enum;
using ExpenseFlow.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ExpenseFlow.Api.Authorization
{
    public abstract class DashboardAuthorized : AuthorizeAttribute, IAuthorizationFilter
    {
        private string permission;
        public DashboardAuthorized(string permission)
        {
            this.permission = permission;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var _context = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
            string token = context.HttpContext.Request.Headers.Authorization.ToString().Split(" ")[1];

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            string role = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            string user = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            string email = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (!Guid.TryParse(user, out var userId))
                throw new UnAuthorizedException(ErrorMessages.UnAuthenticated.ToString());

            if (!Guid.TryParse(role, out Guid roleId))
                throw new UnAuthorizedException(ErrorMessages.UnAuthenticated.ToString());

            var sessionExists = _context.Session.Any(s => s.RefId == userId);
            if (!sessionExists)
                throw new UnAuthorizedException(ErrorMessages.UnAuthenticated.ToString());

            bool hasPermission = false;
            if (_context.User.Any(u => u.Id == userId))
                hasPermission = _context.User
                   .Where(c => c.Id == userId && c.RoleId == roleId)
                   .SelectMany(c => c.Role.RolePermissions)
                   .Select(p => p.Permission.Name)
                   .AsEnumerable()
                   .Any(name => name.Contains(permission));


            //if (!hasPermission)
            //    throw new ForbiddenException(ErrorMessages.Forbidden.ToString());
        }
    }

}
