using ExpenseFlow.Domain.Base;
using ExpenseFlow.Domain.Shared.Enum;
using ExpenseFlow.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace ExpenseFlow.Api.Authorization;

public class DashboardAuthorized
    : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly PermissionType _permission;

    public DashboardAuthorized(PermissionType permission)
    {
        _permission = permission;
    }

    public void OnAuthorization(
        AuthorizationFilterContext context)
    {
        var db =
            context.HttpContext
                .RequestServices
                .GetRequiredService<AppDbContext>();

        string? userClaim =
            context.HttpContext.User
                .FindFirstValue(
                    ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(
                userClaim,
                out Guid userId))
        {
            throw new UnAuthorizedException(
                ErrorMessages.UnAuthenticated.ToString());
        }

        bool sessionExists =
            db.Session.Any(
                session =>
                    session.RefId == userId);

        if (!sessionExists)
        {
            throw new UnAuthorizedException(
                ErrorMessages.UnAuthenticated.ToString());
        }

        bool hasPermission =
            db.User.Any(user =>
                user.Id == userId &&
                user.Role.RolePermissions.Any(
                    rolePermission =>
                        rolePermission.Permission.Code
                        == _permission));

        if (!hasPermission)
        {
            throw new ForbiddenException(
                ErrorMessages.Forbidden.ToString());
        }
    }
}