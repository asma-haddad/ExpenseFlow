using ExpenseFlow.Application.Dto.Auth;
using ExpenseFlow.Application.Services.Helper;
using ExpenseFlow.Application.Services.Token;
using ExpenseFlow.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseFlow.Api.Controllers.ExpenseFlow;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ITokenService _tokenService;

    public AuthController(AppDbContext db, ITokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _db.User
            .Include(x => x.Role)
            .ThenInclude(x => x.RolePermissions)
            .ThenInclude(x => x.Permission)
            .FirstOrDefaultAsync(x => x.Email == request.Email && x.IsActive);

        if (user == null || !PasswordHelper.Verify(request.Password, user.PasswordHash))
            return Unauthorized(new { message = "Invalid email or password." });

        var permissions = user.Role.RolePermissions
            .Where(x => x.IsValid && x.Permission.IsValid)
            .Select(x => x.Permission.Name)
            .Distinct()
            .ToList();

        var token = _tokenService.GenerateToken(user, permissions);

        return Ok(new
        {
            token,
            user = new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                role = user.Role.Name,
                permissions
            }
        });
    }
}