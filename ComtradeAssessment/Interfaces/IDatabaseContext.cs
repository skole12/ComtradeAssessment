using ComtradeAssessment.Entities;
using Microsoft.EntityFrameworkCore;

namespace ComtradeAssessment.Interfaces;

public interface IDatabaseContext
{
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<Campaign> Campaigns { get; }
    DbSet<CampaignOffer> CampaignOffers { get; set; }

    DbSet<TEntity> Set<TEntity>()
        where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
