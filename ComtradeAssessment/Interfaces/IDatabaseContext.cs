using ComtradeAssessment.Entities;
using Microsoft.EntityFrameworkCore;

namespace ComtradeAssessment.Interfaces;

public interface IDatabaseContext
{
    DbSet<User> Users { get; }
    DbSet<TEntity> Set<TEntity>()
        where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
