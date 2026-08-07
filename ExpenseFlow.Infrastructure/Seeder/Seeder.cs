using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ExpenseFlow.Domain.Model.User;
using ExpenseFlow.Infrastructure.Data;

namespace ExpenseFlow.Infrastructure.Seeder;

public static class Seeder
{
    public static async Task SeedData(IServiceProvider services)
    {
        var db = services.GetRequiredService<AppDbContext>();

        var adminRole = await db.Roles.FirstOrDefaultAsync(x => x.Name == "Admin");
        if (adminRole == null)
        {
            adminRole = new Role { Name = "Admin" };
            db.Roles.Add(adminRole);
            await db.SaveChangesAsync();
        }

        var permissionNames = new[]
        {
            "users.read",
            "users.create",
            "users.update",
            "users.delete",
            "roles.manage",
            "permissions.manage"
        };

        foreach (var name in permissionNames)
        {
            if (!await db.Permissions.AnyAsync(x => x.Name == name))
                db.Permissions.Add(new Permission { Name = name });
        }

        await db.SaveChangesAsync();

        var permissions = await db.Permissions.ToListAsync();
        foreach (var permission in permissions)
        {
            var exists = await db.RolePermissions.AnyAsync(x =>
                x.RoleId == adminRole.Id && x.PermissionId == permission.Id);

            if (!exists)
            {
                db.RolePermissions.Add(new RolePermission
                {
                    RoleId = adminRole.Id,
                    PermissionId = permission.Id
                });
            }
        }

        var adminEmail = "admin@expenseflow.local";
        if (!await db.Users.AnyAsync(x => x.Email == adminEmail))
        {
            db.Users.Add(new User
            {
                FirstName = "System",
                LastName = "Admin",
                Email = adminEmail,
                PasswordHash = HashPassword("ChangeMe123!"),
                RoleId = adminRole.Id,
                IsActive = true
            });
        }

        await db.SaveChangesAsync();
    }

    private static string HashPassword(string password)
    {
        const int saltSize = 16;
        const int keySize = 32;
        const int iterations = 100000;

        var salt = RandomNumberGenerator.GetBytes(saltSize);
        var key = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            keySize);

        return $"{iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
    }
}