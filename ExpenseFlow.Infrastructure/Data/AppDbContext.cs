using ExpenseFlow.Domain.Base.Language;
using ExpenseFlow.Domain.Model.AuditLog;
using ExpenseFlow.Domain.Model.Base;
using ExpenseFlow.Domain.Model.Category;
using ExpenseFlow.Domain.Model.Department;
using ExpenseFlow.Domain.Model.Expense;
using ExpenseFlow.Domain.Model.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Reflection;
using System.Text.Json;

namespace ExpenseFlow.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    #region User

    public DbSet<UserModel> User { get; set; }
    public DbSet<RefreshTokenModel> RefreshToken { get; set; }
    public DbSet<RoleModel> Role { get; set; }
    public DbSet<PermissionModel> Permission { get; set; }
    public DbSet<PermissionRoleModel> RolePermission { get; set; }
    public DbSet<SessionModel> Session { get; set; }

    #endregion
    #region
    public DbSet<DepartmentModel> Department { get; set; }
    public DbSet<ExpenseModel> Expense { get; set; }
    public DbSet<CategoryModel> Category { get; set; }

    public DbSet<ExpenseApprovalModel> ExpenseApproval { get; set; }
    #endregion
    #region AuditLog

    public DbSet<AuditLog> AuditLog { get; set; }

    #endregion

    protected override void OnModelCreating(
     ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Ignore<LanguagePropertyModel>();

        ApplyLanguagePropertyConfiguration(modelBuilder);

        ApplyConfigurations(modelBuilder);
        ApplyLanguageDatabaseFunctions(modelBuilder);
        ApplyIsValidQueryFilter(modelBuilder);

        modelBuilder.Entity<UserModel>()
    .HasOne(u => u.Department)
    .WithMany(d => d.Employees)
    .HasForeignKey(u => u.DepartmentId);

        modelBuilder.Entity<DepartmentModel>()
             .HasOne(u => u.Manager).
             WithMany(d => d.ManagedDepartments)
             .HasForeignKey(u => u.ManagerId);
    }

    private static void ApplyConfigurations(
        ModelBuilder modelBuilder)
    {
        #region User



        #endregion

        #region AuditLog

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.ToTable("AuditLog");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Method)
                .HasMaxLength(20);

            entity.Property(x => x.Path)
                .HasMaxLength(1000);

            entity.Property(x => x.IpAddress)
                .HasMaxLength(100);

            entity.Property(x => x.UserId)
                .HasMaxLength(100);
        });

        #endregion
    }

    #region Language property configuration

    /// <summary>
    /// يحوّل جميع خصائص LanguagePropertyModel إلى jsonb
    /// بشكل تلقائي.
    /// </summary>
    private static void ApplyLanguagePropertyConfiguration(
        ModelBuilder modelBuilder)
    {
        var converter =
            new ValueConverter<LanguagePropertyModel, string>(
                value => JsonSerializer.Serialize(
                    value,
                    JsonSerializerOptions.Default),

                json => JsonSerializer
                            .Deserialize<LanguagePropertyModel>(
                                json,
                                JsonSerializerOptions.Default)
                        ?? new LanguagePropertyModel());

        /*
         * هذا الـ comparer مهم حتى يكتشف EF Core
         * التعديلات التي تحدث داخل الـ Dictionary.
         */
        var comparer =
            new ValueComparer<LanguagePropertyModel>(
                (left, right) =>
                    JsonSerializer.Serialize(
                        left,
                        JsonSerializerOptions.Default)
                    ==
                    JsonSerializer.Serialize(
                        right,
                        JsonSerializerOptions.Default),

                value => value == null
                    ? 0
                    : JsonSerializer.Serialize(
                            value,
                            JsonSerializerOptions.Default)
                        .GetHashCode(),

                value => JsonSerializer
                            .Deserialize<LanguagePropertyModel>(
                                JsonSerializer.Serialize(
                                    value,
                                    JsonSerializerOptions.Default),
                                JsonSerializerOptions.Default)
                        ?? new LanguagePropertyModel());

        var entityTypes = modelBuilder.Model
            .GetEntityTypes()
            .ToList();

        foreach (var entityType in entityTypes)
        {
            var languageProperties = entityType.ClrType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property =>
                    property.PropertyType ==
                    typeof(LanguagePropertyModel))
                .ToList();

            foreach (var property in languageProperties)
            {
                var propertyBuilder = modelBuilder
                    .Entity(entityType.ClrType)
                    .Property(property.Name);

                propertyBuilder
                    .HasConversion(converter)
                    .HasColumnType("jsonb");

                propertyBuilder.Metadata
                    .SetValueComparer(comparer);
            }
        }
    }

    #endregion

    #region PostgreSQL language functions

    private static void ApplyLanguageDatabaseFunctions(
        ModelBuilder modelBuilder)
    {
        RegisterLanguageFunction(
            modelBuilder,
            nameof(LanguagePropertyModelExtension.Search),
            "search",
            typeof(LanguagePropertyModel),
            typeof(string));

        RegisterLanguageFunction(
            modelBuilder,
            nameof(LanguagePropertyModelExtension.IsEquals),
            "isequals",
            typeof(LanguagePropertyModel),
            typeof(string));

        RegisterLanguageFunction(
            modelBuilder,
            nameof(LanguagePropertyModelExtension.IsNotEquals),
            "isnotequals",
            typeof(LanguagePropertyModel),
            typeof(string));

        RegisterLanguageFunction(
            modelBuilder,
            nameof(LanguagePropertyModelExtension.StartsWith),
            "startswith",
            typeof(LanguagePropertyModel),
            typeof(string));

        RegisterLanguageFunction(
            modelBuilder,
            nameof(LanguagePropertyModelExtension.EndsWith),
            "endswith",
            typeof(LanguagePropertyModel),
            typeof(string));

        RegisterLanguageFunction(
            modelBuilder,
            nameof(LanguagePropertyModelExtension.IsEmptyVal),
            "isemptyval",
            typeof(LanguagePropertyModel));

        RegisterLanguageFunction(
            modelBuilder,
            nameof(LanguagePropertyModelExtension.IsNotEmptyVal),
            "isnotemptyval",
            typeof(LanguagePropertyModel));

        RegisterLanguageFunction(
            modelBuilder,
            nameof(LanguagePropertyModelExtension.ToDto),
            "todto",
            typeof(LanguagePropertyModel),
            typeof(string));
    }

    private static void RegisterLanguageFunction(
        ModelBuilder modelBuilder,
        string methodName,
        string databaseFunctionName,
        params Type[] parameterTypes)
    {
        var method = typeof(LanguagePropertyModelExtension)
            .GetMethod(
                methodName,
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                types: parameterTypes,
                modifiers: null);

        if (method is null)
        {
            throw new InvalidOperationException(
                $"Language method '{methodName}' was not found.");
        }

        var functionBuilder = modelBuilder
            .HasDbFunction(method)
            .HasName(databaseFunctionName)
            .HasSchema("public");

        /*
         * أول parameter في دوال LanguagePropertyModelExtension
         * اسمه prop ونوع العمود في PostgreSQL هو jsonb.
         */
        functionBuilder
            .HasParameter("prop")
            .HasStoreType("jsonb");
    }

    #endregion

    #region Global query filter

    private static void ApplyIsValidQueryFilter(
        ModelBuilder modelBuilder)
    {
        var baseModelTypes = modelBuilder.Model
            .GetEntityTypes()
            .Where(entityType =>
                typeof(BaseModel)
                    .IsAssignableFrom(entityType.ClrType))
            .Select(entityType => entityType.ClrType)
            .ToList();

        var filterMethod = typeof(AppDbContext)
            .GetMethod(
                nameof(SetIsValidQueryFilter),
                BindingFlags.NonPublic |
                BindingFlags.Static);

        if (filterMethod is null)
        {
            throw new InvalidOperationException(
                "SetIsValidQueryFilter method was not found.");
        }

        foreach (var entityType in baseModelTypes)
        {
            filterMethod
                .MakeGenericMethod(entityType)
                .Invoke(
                    null,
                    new object[]
                    {
                        modelBuilder
                    });
        }
    }

    private static void SetIsValidQueryFilter<TEntity>(
        ModelBuilder modelBuilder)
        where TEntity : BaseModel
    {
        modelBuilder.Entity<TEntity>()
            .HasQueryFilter(entity => entity.IsValid);
    }

    #endregion

    #region Save changes

    public override int SaveChanges()
    {
        FillBaseInfo();

        return base.SaveChanges();
    }

    public override int SaveChanges(
        bool acceptAllChangesOnSuccess)
    {
        FillBaseInfo();

        return base.SaveChanges(
            acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        FillBaseInfo();

        return base.SaveChangesAsync(
            cancellationToken);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        FillBaseInfo();

        return base.SaveChangesAsync(
            acceptAllChangesOnSuccess,
            cancellationToken);
    }

    private void FillBaseInfo()
    {
        var dateTime = DateTime.UtcNow;

        var entries = ChangeTracker
            .Entries<BaseModel>()
            .ToList();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    {
                        entry.Entity.CreatedAt = dateTime;
                        entry.Entity.UpdatedAt = null;
                        entry.Entity.DeletedAt = null;
                        entry.Entity.IsValid = true;

                        break;
                    }

                case EntityState.Modified:
                    {
                        entry.Entity.UpdatedAt = dateTime;

                        entry.Property(x => x.CreatedAt)
                            .IsModified = false;

                        break;
                    }

                case EntityState.Deleted:
                    {
                        // تحويل الحذف الحقيقي إلى Soft Delete.
                        entry.State = EntityState.Modified;

                        entry.Entity.IsValid = false;
                        entry.Entity.DeletedAt = dateTime;
                        entry.Entity.UpdatedAt = dateTime;

                        entry.Property(x => x.CreatedAt)
                            .IsModified = false;

                        break;
                    }
            }
        }
    }

    #endregion
}