using MoodTracker.Application.Abstractions;

namespace MoodTracker.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly MoodTrackerDbContext _dbContext;

    public UnitOfWork(MoodTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
