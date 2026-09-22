using ExpenseFlow.Domain.Base.Language;
using ExpenseFlow.Domain.Model.AuditLog;
using ExpenseFlow.Domain.Model.Base;
using ExpenseFlow.Domain.Model.Category;
using ExpenseFlow.Domain.Model.Department;
using ExpenseFlow.Domain.Model.Expense;
using ExpenseFlow.Domain.Model.User;
using ExpenseFlow.Domain.Shared.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;

namespace ExpenseFlow.Infrastructure.Data;

public class AppDbContext : DbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    private readonly AuditScope _auditScope;
    public AppDbContext(DbContextOptions<AppDbContext> options, AuditScope auditScope, IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _auditScope = auditScope;
        _httpContextAccessor = httpContextAccessor;
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
    public DbSet<EntityPropertyChangeModel> EntityPropertyChanges { get; set; }
    #endregion
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Ignore<LanguagePropertyModel>();
        ApplyLanguagePropertyConfiguration(modelBuilder);
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

    public async Task SaveChangesAuditLogAsync()
    {
        await base.SaveChangesAsync();
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        FillBaseInfo();
        var auditEntries = OnBeforeSaveChanges();
        var result = await base.SaveChangesAsync(cancellationToken);
        await OnAfterSaveChanges(auditEntries);
        return result;
    }
    private List<AuditEntry> OnBeforeSaveChanges()
    {

        var auditEntries = new List<AuditEntry>();

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is AuditLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                continue;

            var auditEntry = new AuditEntry(entry)
            {
                TableName = entry.Metadata.GetTableName(),
                Action = entry.State.ToString()
            };

            bool isSoftDelete = entry.Properties.Any(p => p.Metadata.Name == "IsValid")
                && entry.OriginalValues["IsValid"]?.ToString() == "True"
                && entry.CurrentValues["IsValid"]?.ToString() == "False";

            foreach (var property in entry.Properties)
            {
                string propertyName = property.Metadata.Name;

                if (property.IsTemporary)
                {
                    auditEntry.TemporaryProperties.Add(property);
                    continue;
                }

                var originalValue = property.OriginalValue;
                object newValue;

                if (isSoftDelete && propertyName != "IsValid")
                {
                    newValue = null;
                }
                else
                {
                    newValue = property.CurrentValue;
                }

                auditEntry.OldValues[propertyName] = originalValue;
                auditEntry.NewValues[propertyName] = newValue;
            }

            var entity = entry.Entity;
            var entityType = entity.GetType();

            auditEntries.Add(auditEntry);
        }

        return auditEntries;
    }

    private async Task OnAfterSaveChanges(List<AuditEntry> auditEntries)
    {
        foreach (var auditEntry in auditEntries)
        {
            var entry = auditEntry.Entry;

            if (entry.Entity is AuditLog) continue;

            var auditLog = new AuditLog
            {
                EntityName = auditEntry.TableName,
                AuditLogEventType = auditEntry.Action switch
                {
                    "Added" => EventType.Added,
                    "Modified" => EventType.Modified,
                    "Deleted" => EventType.Deleted,
                },
                EntityId = Guid.Parse(entry.Properties.First(p => p.Metadata.IsPrimaryKey()).CurrentValue?.ToString()),
                UserId = GetUserId()
            };

            foreach (var propertyName in auditEntry.OldValues.Keys.Union(auditEntry.NewValues.Keys))
            {
                var oldValue = auditEntry.OldValues.ContainsKey(propertyName) ? auditEntry.OldValues[propertyName]?.ToString() : null;
                var newValue = auditEntry.NewValues.ContainsKey(propertyName) ? auditEntry.NewValues[propertyName]?.ToString() : null;

                bool shouldLogChange = auditEntry.Action switch
                {
                    "Added" => newValue != null,
                    "Deleted" => oldValue != null,
                    "Modified" => !Equals(oldValue, newValue),
                    _ => false
                };

                if (shouldLogChange)
                {
                    var property = entry.Properties.FirstOrDefault(p => p.Metadata.Name == propertyName);

                    auditLog.EntityPropertyChanges.Add(new EntityPropertyChangeModel
                    {
                        PropertyName = propertyName,
                        OriginalValue = auditEntry.Action == "Added" ? null : oldValue,
                        NewValue = auditEntry.Action == "Deleted" ? null : newValue,
                        PropertyTypeFullName = property?.Metadata.ClrType.FullName
                    });
                }
            }

            _auditScope.Logs.Add(auditLog);
        }
    }

    public class AuditEntry
    {
        public AuditEntry(EntityEntry entry)
        {
            Entry = entry;
        }

        public EntityEntry Entry { get; }
        public string Action { get; set; }
        public string TableName { get; set; }
        public Dictionary<string, object> OldValues { get; } = new();
        public Dictionary<string, object> NewValues { get; } = new();
        public List<PropertyEntry> TemporaryProperties { get; } = new();


        public bool HasTemporaryProperties => TemporaryProperties.Any();
    }

    private Guid GetUserId()
    {
        var id = Guid.Empty;
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity is { IsAuthenticated: true })
            {
                id = Guid.Parse(httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
            }
        }
        catch (Exception)
        {
            id = Guid.Empty;
        }

        return id;
    }

    #endregion

    private void FillBaseInfo()
    {
        var now = DateTime.UtcNow;
        var userId = GetUserId();

        foreach (var entry in ChangeTracker.Entries<BaseModel>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedAt = now;
                entry.Entity.CreatedBy = userId;
                entry.Entity.UpdatedBy = userId;
            }
            else if (entry.State == EntityState.Modified)
            {
                var isValidProperty = entry.Property(x => x.IsValid);

                var isSoftDelete =
                    isValidProperty.IsModified &&
                    isValidProperty.OriginalValue == true &&
                    isValidProperty.CurrentValue == false;

                if (isSoftDelete)
                {
                    entry.Entity.DeletedAt = now;
                    entry.Entity.DeletedBy = userId;
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.UpdatedBy = userId;
                }
                else
                {
                    entry.Property(x => x.CreatedAt).IsModified = false;
                    entry.Property(x => x.CreatedBy).IsModified = false;

                    entry.Entity.UpdatedAt = now;
                    entry.Entity.UpdatedBy = userId;
                }
            }
        }
    }
}

