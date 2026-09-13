using ExpenseFlow.Domain.Model.User;
using ExpenseFlow.Domain.Shared.Enum;
using ExpenseFlow.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExpenseFlow.Infrastructure.Seeder;

public static class SeedMethods
{
    // =====================================================
    // Permissions
    // =====================================================

    public static async Task SeedPermissions(AppDbContext context)
    {
        var existingPermissions =
            await context.Permission
                .Select(p => p.Code)
                .ToHashSetAsync();

        var permissionTypes =
            Enum.GetValues<PermissionType>();

        var newPermissions =
            new List<PermissionModel>();

        foreach (var permissionType in permissionTypes)
        {
            if (existingPermissions.Contains(permissionType))
            {
                continue;
            }

            newPermissions.Add(
                new PermissionModel
                {
                    Id = Guid.NewGuid(),
                    Code = permissionType,
                    Name = permissionType.ToString()
                });
        }

        if (newPermissions.Any())
        {
            await context.Permission
                .AddRangeAsync(newPermissions);

            await context.SaveChangesAsync();
        }
    }


    // =====================================================
    // Roles
    // =====================================================

    public static async Task SeedRoles(AppDbContext context)
    {
        var roles = new[]
        {
            new
            {
                RoleType = RoleType.Employee,
                Name = "Employee"
            },

            new
            {
                RoleType = RoleType.Manager,
                Name = "Manager"
            },

            new
            {
                RoleType = RoleType.Finance,
                Name = "Finance"
            },

            new
            {
                RoleType = RoleType.Admin,
                Name = "Admin"
            }
        };

        foreach (var roleData in roles)
        {
            var existingRole =
                await context.Role
                    .FirstOrDefaultAsync(
                        r => r.RoleType == roleData.RoleType);

            if (existingRole != null)
            {
                existingRole.Name =
                    roleData.Name;

                continue;
            }

            var role =
                new RoleModel
                {
                    Id = Guid.NewGuid(),
                    RoleType = roleData.RoleType,
                    Name = roleData.Name
                };

            context.Role.Add(role);
        }

        await context.SaveChangesAsync();
    }


    // =====================================================
    // Role Permissions
    // =====================================================

    public static async Task SeedRolePermissions(
        AppDbContext context)
    {
        var roles =
            await context.Role
                .ToDictionaryAsync(
                    r => r.RoleType);

        var permissions =
            await context.Permission
                .ToDictionaryAsync(
                    p => p.Code);

        var rolePermissionMap =
            new Dictionary<RoleType, PermissionType[]>
            {
                [RoleType.Employee] =
                    PermissionGroups.GetEmployeePermissions(),

                [RoleType.Manager] =
                    PermissionGroups.GetManagerPermissions(),

                [RoleType.Finance] =
                    PermissionGroups.GetFinancePermissions(),

                [RoleType.Admin] =
                    PermissionGroups.GetAdminPermissions()
            };

        var existingLinks =
            await context.RolePermission
                .AsNoTracking()
                .Select(x => new
                {
                    x.RoleId,
                    x.PermissionId
                })
                .ToListAsync();

        var existingLinkSet =
            existingLinks
                .Select(x =>
                    (
                        x.RoleId,
                        x.PermissionId
                    ))
                .ToHashSet();

        var newLinks =
            new List<PermissionRoleModel>();

        foreach (var roleEntry in rolePermissionMap)
        {
            var role =
                roles[roleEntry.Key];

            foreach (var permissionType
                     in roleEntry.Value.Distinct())
            {
                var permission =
                    permissions[permissionType];

                var linkKey =
                    (
                        RoleId: role.Id,
                        PermissionId: permission.Id
                    );

                if (existingLinkSet.Contains(linkKey))
                {
                    continue;
                }

                newLinks.Add(
                    new PermissionRoleModel
                    {
                        RoleId = role.Id,
                        PermissionId = permission.Id
                    });

                existingLinkSet.Add(linkKey);
            }
        }

        if (newLinks.Any())
        {
            await context.RolePermission
                .AddRangeAsync(newLinks);

            await context.SaveChangesAsync();
        }
    }


    // =====================================================
    // Admin User
    // =====================================================

    public static async Task SeedAdminUser(
        AppDbContext context)
    {
        const string adminEmail =
            "admin@gmail.com";

        const string adminPassword =
            "Aaa@1234";

        var adminExists =
            await context.User
                .AnyAsync(
                    u => u.Email == adminEmail);

        if (adminExists)
        {
            return;
        }

        var adminRole =
            await context.Role
                .FirstOrDefaultAsync(
                    r => r.RoleType == RoleType.Admin);

        if (adminRole == null)
        {
            return;
        }

        var adminUser =
            new UserModel
            {
                Id = Guid.NewGuid(),
                FirstName = "System",
                LastName = "Admin",
                Email = adminEmail,
                RoleId = adminRole.Id
            };

        var passwordHasher =
            new PasswordHasher<UserModel>();

        adminUser.PasswordHash =
            passwordHasher.HashPassword(
                adminUser,
                adminPassword);

        context.User.Add(adminUser);

        await context.SaveChangesAsync();
    }
}
