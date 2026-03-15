using Microsoft.EntityFrameworkCore;
using MoodTracker.Domain.Abstractions;
using MoodTracker.Domain.Entities;

namespace MoodTracker.Infrastructure.Persistence;

public class MoodTrackerDbContext : DbContext
{
    public MoodTrackerDbContext(DbContextOptions<MoodTrackerDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<MoodEntry> MoodEntries => Set<MoodEntry>();

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditing();
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MoodTrackerDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    private void ApplyAuditing()
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAtUtc = utcNow;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAtUtc = utcNow;
            }
        }
    }
}
