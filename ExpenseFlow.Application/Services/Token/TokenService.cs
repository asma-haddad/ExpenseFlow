using ExpenseFlow.Domain.Model.User;
using ExpenseFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ExpenseFlow.Application.Services.Token;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _context;

    public TokenService(IConfiguration configuration, AppDbContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    private int AccessMinutes =>
        int.TryParse(_configuration["JWT:DurationInMinute"], out var m) ? m : 60;

    private int RefreshDays =>
        int.TryParse(_configuration["JWT:RefreshTokenMinutes"], out var d) ? d : 30;

    public async Task<TokenDto> IssueTokensAsync(Guid userId, string? ip, string? userAgent, bool isDevice)
    {
        var (jwt, accessExpUtc) = await CreateJwtAsync(userId);

        var (entity, plain) = CreateRefreshToken(userId, ip, userAgent, RefreshDays, isDevice);
        _context.RefreshToken.Add(entity);
        await _context.SaveChangesAsync();

        return new TokenDto
        {
            Token = jwt,
            RefreshToken = plain,
            ExpiresAtUtc = accessExpUtc,
            RefreshTokenExpiresAtUtc = entity.ExpiresAtUtc
        };
    }

    // ===  Refresh Token ===
    public async Task<TokenDto> RefreshAsync(string refreshToken, string? ip, string? userAgent, bool isDevice)
    {
        var hash = Sha256(refreshToken);
        var existing = await _context.RefreshToken.AsTracking()
            .FirstOrDefaultAsync(x => x.TokenHash == hash);

        if (existing == null || !existing.IsActive)
            throw new SecurityTokenException("Invalid or expired refresh token");

        if (!string.IsNullOrEmpty(existing.ReplacedByTokenHash))
            throw new SecurityTokenException("Refresh token already rotated.");

        var (jwt, accessExpUtc) = await CreateJwtAsync(existing.UserId);

        var (newEntity, newPlain) = CreateRefreshToken(existing.UserId, ip, userAgent, RefreshDays, isDevice);

        existing.RevokedAtUtc = DateTime.UtcNow;
        existing.RevokedByIp = ip;
        existing.ReplacedByTokenHash = newEntity.TokenHash;

        _context.RefreshToken.Add(newEntity);
        await _context.SaveChangesAsync();

        return new TokenDto
        {
            Token = jwt,
            RefreshToken = newPlain,
            ExpiresAtUtc = accessExpUtc,
            RefreshTokenExpiresAtUtc = newEntity.ExpiresAtUtc
        };
    }

    public async Task RevokeAsync(string refreshToken, string? ip, string? reason = null)
    {
        var hash = Sha256(refreshToken);
        var existing = await _context.RefreshToken.AsTracking()
            .FirstOrDefaultAsync(x => x.TokenHash == hash);

        if (existing == null || existing.RevokedAtUtc != null) return;

        existing.RevokedAtUtc = DateTime.UtcNow;
        existing.RevokedByIp = ip;
        await _context.SaveChangesAsync();
    }

    private async Task<(string jwt, DateTime expiresUtc)> CreateJwtAsync(Guid userId)
    {
        var user = await _context.User.AsNoTracking().FirstOrDefaultAsync(s => s.Id == userId)
                     ?? throw new Exception("Client not found.");

        var role = await _context.Role.AsNoTracking().FirstOrDefaultAsync(s => s.Id == user.RoleId)
                   ?? throw new Exception("Role not found.");

        var issuer = _configuration["JWT:Issuer"] ?? throw new InvalidOperationException("JWT:Issuer missing");
        var audience = _configuration["JWT:Audience"] ?? throw new InvalidOperationException("JWT:Audience missing");
        var keyB64 = _configuration["JWT:Key"] ?? throw new InvalidOperationException("JWT:Key missing");

        byte[] keyBytes;
        try { keyBytes = Convert.FromBase64String(keyB64); }
        catch { keyBytes = Encoding.UTF8.GetBytes(keyB64); }
        if (keyBytes.Length < 32)
            throw new InvalidOperationException("JWT key must be >= 256 bits.");

        var signingKey = new SymmetricSecurityKey(keyBytes);
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var now = DateTime.UtcNow;
        var expires = now.AddMinutes(AccessMinutes);

        var claims = new[]
        {
                //new Claim(JwtRegisteredClaimNames.Sub, client.Id.ToString()),
                //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                //new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),

                new Claim(ClaimTypes.Role, role.Id.ToString()),
                //new Claim(ClaimTypes.Actor, ActorType.Client.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.GivenName, user.FirstName ?? string.Empty),
                new Claim(ClaimTypes.Surname, user.LastName ?? string.Empty),
                //new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty)
            };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: now,
            expires: expires,
            signingCredentials: credentials);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return (jwt, expires);
    }

    private (RefreshTokenModel entity, string plain) CreateRefreshToken(Guid clientId, string? ip, string? userAgent, int ttlMinutes, bool isDevice)
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        var plain = Convert.ToBase64String(bytes);
        var hash = Sha256(plain);
        if (isDevice) ttlMinutes = ttlMinutes * 2;
        var entity = new RefreshTokenModel
        {
            UserId = clientId,
            TokenHash = hash,
            CreatedAt = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(ttlMinutes),
            CreatedByIp = ip,
            UserAgent = userAgent
        };
        return (entity, plain);
    }

    private static string Sha256(string input)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes);
    }

    public string GenerateToken(UserModel user, IEnumerable<string> permissions)
    {
        throw new NotImplementedException();
    }
}

