using ExpenseFlow.Domain.Base.Language;
using ExpenseFlow.Domain.Model.User;
using ExpenseFlow.Domain.Shared.Enum;
using ExpenseFlow.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseFlow.Infrastructure.Seeder;

public static class Seeder
{
    private sealed record RoleSeedDefinition(
        RoleType RoleType,
        string EnglishName,
        string ArabicName);


    private static readonly RoleSeedDefinition[] RoleDefinitions =
    {
        new(
            RoleType.Employee,
            "Employee",
            "موظف"),

        new(
            RoleType.Manager,
            "Manager",
            "مدير"),

        new(
            RoleType.Finance,
            "Finance",
            "مالية"),

        new(
            RoleType.Admin,
            "Admin",
            "مدير النظام")
    };


    public static async Task SeedData(
        IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        using IServiceScope scope =
            services.CreateScope();

        AppDbContext db =
            scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

        IConfiguration configuration =
            scope.ServiceProvider
                .GetRequiredService<IConfiguration>();

        ValidateRoles();

        await using var transaction =
            await db.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            Dictionary<RoleType, RoleModel> roles =
                await SeedRolesAsync(
                    db,
                    cancellationToken);

            Dictionary<PermissionType, PermissionModel>
                permissions =
                    await SeedPermissionsAsync(
                        db,
                        cancellationToken);

            await SeedRolePermissionsAsync(
                db,
                roles,
                permissions,
                cancellationToken);

            await SeedAdminUserAsync(
                db,
                roles,
                configuration,
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken);

            throw;
        }
    }


    private static async Task<
        Dictionary<RoleType, RoleModel>>
        SeedRolesAsync(
            AppDbContext db,
            CancellationToken cancellationToken)
    {
        Dictionary<RoleType, RoleModel> roles =
            await db.Role
                .ToDictionaryAsync(
                    x => x.RoleType,
                    cancellationToken);

        foreach (RoleSeedDefinition definition
                 in RoleDefinitions)
        {
            LanguagePropertyModel name =
                new()
                {
                    ["en"] = definition.EnglishName,
                    ["ar"] = definition.ArabicName
                };

            if (roles.TryGetValue(
                    definition.RoleType,
                    out RoleModel? existingRole))
            {
                existingRole.Name = name;
                continue;
            }

            var role = new RoleModel
            {
                Id = Guid.NewGuid(),

                RoleType = definition.RoleType,

                Name = name
            };

            db.Role.Add(role);

            roles.Add(
                definition.RoleType,
                role);
        }

        await db.SaveChangesAsync(
            cancellationToken);

        return roles;
    }


    private static async Task<
        Dictionary<PermissionType, PermissionModel>>
        SeedPermissionsAsync(
            AppDbContext db,
            CancellationToken cancellationToken)
    {
        Dictionary<PermissionType, PermissionModel>
            permissions =
                await db.Permission
                    .ToDictionaryAsync(
                        x => x.Code,
                        cancellationToken);

        foreach (PermissionType permissionType in Enum.GetValues<PermissionType>())
        {
            string permissionName = permissionType.ToString();

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


    private static async Task SeedRolePermissionsAsync(
        AppDbContext db,
        IReadOnlyDictionary<RoleType, RoleModel> roles,
        IReadOnlyDictionary<
            PermissionType,
            PermissionModel> permissions,
        CancellationToken cancellationToken)
    {
        Dictionary<
            RoleType,
            PermissionType[]> rolePermissionMap =
                CreateRolePermissionMap();


        var existingData =
            await db.RolePermission
                .AsNoTracking()
                .Select(x => new
                {
                    x.RoleId,
                    x.PermissionId
                })
                .ToListAsync(
                    cancellationToken);


        HashSet<(Guid RoleId, Guid PermissionId)>
            existingLinks =
                existingData
                    .Select(x => (
                        x.RoleId,
                        x.PermissionId))
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
                PermissionModel permission =
                    permissions[permissionType];


                var key =
                    (
                        role.Id,
                        permission.Id
                    );


                if (!existingLinks.Add(key))
                {
                    continue;
                }


                newLinks.Add(
                    new PermissionRoleModel
                    {
                        RoleId =
                            role.Id,

                        PermissionId =
                            permission.Id
                    });
            }
        }


        if (newLinks.Count == 0)
        {
            return;
        }


        await db.RolePermission.AddRangeAsync(
            newLinks,
            cancellationToken);

        await db.SaveChangesAsync(
            cancellationToken);
    }


    private static Dictionary<
        RoleType,
        PermissionType[]> CreateRolePermissionMap()
    {
        PermissionType[] employeePermissions =
        {
            PermissionType.ExpenseViewOwn,
            PermissionType.ExpenseCreate,
            PermissionType.ExpenseEditOwnDraft,
            PermissionType.ExpenseDeleteOwnDraft,
            PermissionType.ExpenseSubmit
        };


        PermissionType[] managerOnlyPermissions =
        {
            PermissionType.ExpenseViewDepartment,
            PermissionType.ExpenseApprove,
            PermissionType.ExpenseReject
        };


        PermissionType[] financeOnlyPermissions =
        {
            PermissionType.ExpenseViewApproved,
            PermissionType.ExpenseMarkAsPaid,
            PermissionType.ExpenseViewReports
        };


        PermissionType[] managerPermissions =
            employeePermissions
                .Concat(managerOnlyPermissions)
                .Distinct()
                .ToArray();


        PermissionType[] financePermissions =
            employeePermissions
                .Concat(financeOnlyPermissions)
                .Distinct()
                .ToArray();


        PermissionType[] adminPermissions =
            Enum.GetValues<PermissionType>();


        return new Dictionary<
            RoleType,
            PermissionType[]>
        {
            [RoleType.Employee] =
                employeePermissions,

            [RoleType.Manager] =
                managerPermissions,

            [RoleType.Finance] =
                financePermissions,

            [RoleType.Admin] =
                adminPermissions
        };
    }


    private static async Task SeedAdminUserAsync(
        AppDbContext db,
        IReadOnlyDictionary<
            RoleType,
            RoleModel> roles,
        IConfiguration configuration,
        CancellationToken cancellationToken)
    {
        string adminEmail =
            configuration["Seed:AdminEmail"]
            ?? "admin@expenseflow.local";


        bool exists =
            await db.User.AnyAsync(
                x => x.Email == adminEmail,
                cancellationToken);


        if (exists)
        {
            return;
        }


        string? adminPassword =
            configuration["Seed:AdminPassword"];


        if (string.IsNullOrWhiteSpace(
                adminPassword))
        {
            throw new InvalidOperationException(
                "Seed:AdminPassword is not configured.");
        }


        RoleModel adminRole =
            roles[RoleType.Admin];


        var adminUser =
            new UserModel
            {
                Id = Guid.NewGuid(),

                FirstName =
                    "System",

                LastName =
                    "Admin",

                Email =
                    adminEmail,

                RoleId =
                    adminRole.Id,

                IsActive =
                    true
            };


        var passwordHasher =
            new PasswordHasher<UserModel>();


        // adminUser.PasswordHash =
        passwordHasher.HashPassword(
            adminUser,
            adminPassword);


        await db.User.AddAsync(
            adminUser,
            cancellationToken);

        await db.SaveChangesAsync(
            cancellationToken);
    }


    private static void ValidateRoles()
    {
        RoleType[] definedRoles =
            RoleDefinitions
                .Select(x => x.RoleType)
                .ToArray();


        RoleType[] duplicateRoles =
            definedRoles
                .GroupBy(x => x)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
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
                .Except(definedRoles)
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