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
    public DbSet<Role> Roles { get; set; }
    public DbSet<Campaign> Campaigns { get; set; }
    public DbSet<CampaignOffer> CampaignOffers { get; set; }
    public DbSet<BackgroundJobStatus> BackgroundJobStatuses { get; set; }

    #endregion

    public new DbSet<TEntity> Set<TEntity>()
        where TEntity : class => base.Set<TEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().Property(e => e.IsActive).HasDefaultValue(true);
        modelBuilder.Entity<Campaign>().Property(e => e.IsActive).HasDefaultValue(true);
        modelBuilder.Entity<Campaign>().Property(e => e.ResultsConcluded).HasDefaultValue(false);
    }
}
