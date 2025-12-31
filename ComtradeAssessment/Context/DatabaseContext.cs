using ComtradeAssessment.Entities;
using ComtradeAssessment.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ComtradeAssessment.Context;

public class DatabaseContext(DbContextOptions<DatabaseContext> options)
    : DbContext(options),
        IDatabaseContext
{
    #region DbSets
    public DbSet<User> Users { get; set; }

    #endregion

    public new DbSet<TEntity> Set<TEntity>()
        where TEntity : class => base.Set<TEntity>();
}
