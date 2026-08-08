using ExpenseFlow.Domain.Model.AuditLog;
using ExpenseFlow.Domain.Model.Base;
using ExpenseFlow.Domain.Model.User;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ExpenseFlow.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    #region User

    public DbSet<UserModel> User { get; set; }
    public DbSet<Role> Role { get; set; }
    public DbSet<Permission> Permission { get; set; }
    public DbSet<RolePermission> RolePermission { get; set; }
    public DbSet<SessionModel> Session { get; set; }
    #endregion

    #region AuditLog

    public DbSet<AuditLog> AuditLog { get; set; }

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ApplyConfigurations(modelBuilder);
        ApplyIsValidQueryFilter(modelBuilder);
    }

    private static void ApplyConfigurations(ModelBuilder modelBuilder)
    {
        #region User

        modelBuilder.Entity<UserModel>(entity =>
        {
            entity.ToTable("User");

            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.Email)
                .IsUnique();

            entity.Property(x => x.Email)
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(x => x.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.LastName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.PasswordHash)
                .IsRequired();

            entity.HasOne(x => x.Role)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.Name)
                .IsUnique();

            entity.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.ToTable("Permission");

            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.Name)
                .IsUnique();

            entity.Property(x => x.Name)
                .HasMaxLength(150)
                .IsRequired();
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.ToTable("RolePermission");

            entity.HasKey(x => x.Id);

            entity.HasIndex(x => new
            {
                x.RoleId,
                x.PermissionId
            }).IsUnique();

            entity.HasOne(x => x.Role)
                .WithMany(x => x.RolePermissions)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Permission)
                .WithMany(x => x.RolePermissions)
                .HasForeignKey(x => x.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

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

    private static void ApplyIsValidQueryFilter(ModelBuilder modelBuilder)
    {
        var baseModelTypes = modelBuilder.Model
            .GetEntityTypes()
            .Where(entityType =>
                typeof(BaseModel).IsAssignableFrom(entityType.ClrType))
            .Select(entityType => entityType.ClrType)
            .ToList();

        var filterMethod = typeof(AppDbContext)
            .GetMethod(
                nameof(SetIsValidQueryFilter),
                BindingFlags.NonPublic | BindingFlags.Static);

        if (filterMethod is null)
        {
            throw new InvalidOperationException(
                "SetIsValidQueryFilter method was not found.");
        }

        foreach (var entityType in baseModelTypes)
        {
            filterMethod
                .MakeGenericMethod(entityType)
                .Invoke(null, new object[] { modelBuilder });
        }
    }

    private static void SetIsValidQueryFilter<TEntity>(
        ModelBuilder modelBuilder)
        where TEntity : BaseModel
    {
        modelBuilder.Entity<TEntity>()
            .HasQueryFilter(entity => entity.IsValid);
    }

    public override int SaveChanges()
    {
        FillBaseInfo();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        FillBaseInfo();

        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        FillBaseInfo();

        return base.SaveChangesAsync(cancellationToken);
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
}