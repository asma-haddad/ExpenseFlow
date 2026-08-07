using Microsoft.EntityFrameworkCore;
using ExpenseFlow.Domain.Model.AuditLog;
using ExpenseFlow.Domain.Model.Base;
using ExpenseFlow.Domain.Model.User;

namespace ExpenseFlow.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasQueryFilter(x => x.IsValid);
            entity.HasIndex(x => x.Email).IsUnique();
            entity.Property(x => x.Email).HasMaxLength(255).IsRequired();
            entity.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(x => x.LastName).HasMaxLength(100).IsRequired();
            entity.Property(x => x.PasswordHash).IsRequired();

            entity.HasOne(x => x.Role)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasQueryFilter(x => x.IsValid);
            entity.HasIndex(x => x.Name).IsUnique();
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasQueryFilter(x => x.IsValid);
            entity.HasIndex(x => x.Name).IsUnique();
            entity.Property(x => x.Name).HasMaxLength(150).IsRequired();
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasQueryFilter(x => x.IsValid);
            entity.HasIndex(x => new { x.RoleId, x.PermissionId }).IsUnique();

            entity.HasOne(x => x.Role)
                .WithMany(x => x.RolePermissions)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Permission)
                .WithMany(x => x.RolePermissions)
                .HasForeignKey(x => x.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasQueryFilter(x => x.IsValid);
            entity.Property(x => x.Method).HasMaxLength(20);
            entity.Property(x => x.Path).HasMaxLength(1000);
            entity.Property(x => x.IpAddress).HasMaxLength(100);
            entity.Property(x => x.UserId).HasMaxLength(100);
        });
    }

    public override int SaveChanges()
    {
        ApplyBaseModelRules();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyBaseModelRules();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyBaseModelRules()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseModel>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.IsValid = true;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
            }
            else if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsValid = false;
                entry.Entity.DeletedAt = now;
                entry.Entity.UpdatedAt = now;
            }
        }
    }
}