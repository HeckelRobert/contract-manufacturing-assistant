namespace QuotationAccelerator.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ProjectIndexEntity> Projects => Set<ProjectIndexEntity>();

    public DbSet<AppSettingEntity> Settings => Set<AppSettingEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProjectIndexEntity>(entity =>
        {
            entity.ToTable("projects");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.ProjectNumber);
        });

        modelBuilder.Entity<AppSettingEntity>(entity =>
        {
            entity.ToTable("settings");
            entity.HasKey(x => x.Key);
        });
    }
}
