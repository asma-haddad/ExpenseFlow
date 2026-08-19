using ExpenseFlow.Domain.Model.User;
using ExpenseFlow.Domain.Shared.Enum;
using ExpenseFlow.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseFlow.Infrastructure.Seeder;

public static class Seeder
{
    private sealed record RoleSeedDefinition(RoleType RoleType, string Name);

    private static readonly RoleSeedDefinition[] RoleDefinitions =
    {
        new(RoleType.Employee, "Employee"),
        new(RoleType.Manager, "Manager"),
        new(RoleType.Finance, "Finance"),
        new(RoleType.Admin, "Admin")
    };


    public static async Task SeedData(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using IServiceScope scope = services.CreateScope();

        AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        ValidateRoles();

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            Dictionary<RoleType, RoleModel> roles = await SeedRolesAsync(db, cancellationToken);

            Dictionary<PermissionType, PermissionModel>
                permissions = await SeedPermissionsAsync(db, cancellationToken);

            await SeedRolePermissionsAsync(db, roles, permissions, cancellationToken);

            await SeedAdminUserAsync(db, roles, cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
    // =====================================================
    // Roles
    // =====================================================

    private static async Task<
        Dictionary<RoleType, RoleModel>> SeedRolesAsync(AppDbContext db, CancellationToken cancellationToken)
    {
        Dictionary<RoleType, RoleModel> roles = await db.Role.ToDictionaryAsync(role => role.RoleType, cancellationToken);

        foreach (RoleSeedDefinition definition in RoleDefinitions)
        {
            if (roles.TryGetValue(definition.RoleType, out RoleModel? existingRole))
            {
                existingRole.Name = definition.Name;
                continue;
            }
            var role = new RoleModel
            {
                Id = Guid.NewGuid(),
                RoleType = definition.RoleType,
                Name = definition.Name
            };

            db.Role.Add(role);
            roles.Add(definition.RoleType, role);
        }
        await db.SaveChangesAsync(cancellationToken);
        return roles;
    }
    // =====================================================
    // Permissions
    // =====================================================

    private static async Task<Dictionary<PermissionType, PermissionModel>>
        SeedPermissionsAsync(AppDbContext db, CancellationToken cancellationToken)
    {
        Dictionary<PermissionType, PermissionModel>
            permissions = await db.Permission.ToDictionaryAsync(permission => permission.Code, cancellationToken);

        PermissionType[] permissionTypes = Enum.GetValues<PermissionType>();
        foreach (PermissionType permissionType in permissionTypes)
        {
            string permissionName =
                permissionType.ToString();

            if (permissions.TryGetValue(
                    permissionType,
                    out PermissionModel? existingPermission))
            {
                existingPermission.Name =
                    permissionName;

                continue;
            }

            var permission =
                new PermissionModel
                {
                    Id = Guid.NewGuid(),
                    Code = permissionType,
                    Name = permissionName
                };

            db.Permission.Add(permission);

            permissions.Add(
                permissionType,
                permission);
        }

        await db.SaveChangesAsync(
            cancellationToken);

        return permissions;
    }
    // =====================================================
    // Role - Permission
    // =====================================================
    private static async Task SeedRolePermissionsAsync(
        AppDbContext db,
        IReadOnlyDictionary<RoleType, RoleModel> roles,
        IReadOnlyDictionary<
            PermissionType,
            PermissionModel> permissions,
        CancellationToken cancellationToken)
    {
        Dictionary<RoleType, PermissionType[]> rolePermissionMap = CreateRolePermissionMap();

        var existingLinkData =
            await db.RolePermission
                .AsNoTracking()
                .Select(link => new
                {
                    link.RoleId,
                    link.PermissionId
                })
                .ToListAsync(
                    cancellationToken);

        HashSet<(Guid RoleId, Guid PermissionId)>
            existingLinkSet =
                existingLinkData
                    .Select(link => (
                        link.RoleId,
                        link.PermissionId))
                    .ToHashSet();

        List<PermissionRoleModel> newLinks =
            new();

        foreach (var roleEntry
                 in rolePermissionMap)
        {
            RoleModel role =
                roles[roleEntry.Key];

            foreach (PermissionType permissionType
                     in roleEntry.Value.Distinct())
            {
                if (!permissions.TryGetValue(
                        permissionType,
                        out PermissionModel? permission))
                {
                    throw new InvalidOperationException(
                        $"Permission '{permissionType}' was not found.");
                }

                var linkKey =
                    (
                        RoleId: role.Id,
                        PermissionId: permission.Id
                    );

                if (!existingLinkSet.Add(linkKey))
                {
                    continue;
                }
                newLinks.Add(new PermissionRoleModel
                {
                    RoleId = role.Id,
                    PermissionId = permission.Id
                });
            }
        }

        if (newLinks.Count == 0)
        {
            return;
        }

        await db.RolePermission
            .AddRangeAsync(
                newLinks,
                cancellationToken);

        await db.SaveChangesAsync(
            cancellationToken);
    }


    // =====================================================
    // Role Permission Map
    // =====================================================

    private static Dictionary<
        RoleType,
        PermissionType[]>
        CreateRolePermissionMap()
    {
        return new Dictionary<
            RoleType,
            PermissionType[]>
        {
            [RoleType.Employee] =
                PermissionGroups
                    .GetEmployeePermissions(),

            [RoleType.Manager] =
                PermissionGroups
                    .GetManagerPermissions(),

            [RoleType.Finance] =
                PermissionGroups
                    .GetFinancePermissions(),

            [RoleType.Admin] =
                PermissionGroups
                    .GetAdminPermissions()
        };
    }


    // =====================================================
    // Admin User
    // =====================================================

    private static async Task SeedAdminUserAsync(
        AppDbContext db,
        IReadOnlyDictionary<
            RoleType,
            RoleModel> roles,
        CancellationToken cancellationToken)
    {
        const string adminEmail =
            "admin@expenseflow.local";

        const string adminPassword =
            "Admin@123";

        bool adminExists =
            await db.User
                .AnyAsync(
                    user =>
                        user.Email == adminEmail,
                    cancellationToken);

        if (adminExists)
        {
            return;
        }

        RoleModel adminRole =
            roles[RoleType.Admin];

        var adminUser =
            new UserModel
            {
                Id = Guid.NewGuid(),

                FirstName = "System",
                LastName = "Admin",

                Email = adminEmail,

                RoleId = adminRole.Id,

            };

        var passwordHasher =
            new PasswordHasher<UserModel>();

        adminUser.PasswordHash =
            passwordHasher.HashPassword(
                adminUser,
                adminPassword);

        await db.User.AddAsync(
            adminUser,
            cancellationToken);

        await db.SaveChangesAsync(
            cancellationToken);
    }


    // =====================================================
    // Validation
    // =====================================================

    private static void ValidateRoles()
    {
        RoleType[] definedRoles =
            RoleDefinitions
                .Select(definition =>
                    definition.RoleType)
                .ToArray();

        RoleType[] duplicateRoles =
            definedRoles
                .GroupBy(roleType =>
                    roleType)
                .Where(group =>
                    group.Count() > 1)
                .Select(group =>
                    group.Key)
                .ToArray();

        if (duplicateRoles.Length > 0)
        {
            throw new InvalidOperationException(
                "Duplicated role definitions: " +
                string.Join(
                    ", ",
                    duplicateRoles));
        }

        RoleType[] missingRoles =
            Enum.GetValues<RoleType>()
                .Except(
                    definedRoles)
                .ToArray();

        if (missingRoles.Length > 0)
        {
            throw new InvalidOperationException(
                "Missing role definitions: " +
                string.Join(
                    ", ",
                    missingRoles));
        }
    }
}