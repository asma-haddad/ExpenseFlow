using ExpenseFlow.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseFlow.Infrastructure.Seeder;

public static class Seeder
{
    public static async Task SeedData(IServiceProvider serviceProvider)
    {
        var context =
            serviceProvider.GetService<AppDbContext>();

        await SeedMethods.SeedPermissions(context);
        await SeedMethods.SeedRoles(context);
        await SeedMethods.SeedRolePermissions(context);
        await SeedMethods.SeedAdminUser(context);

        context.SaveChanges();
    }
}