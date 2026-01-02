using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ComtradeAssessment.Entities;

[PrimaryKey(nameof(Id))]
public class CampaignOffer
{
    public long Id { get; set; }
    public int CampaignId { get; set; }
    public Guid AgentId { get; set; }

    [Required]
    public int CustomerId { get; set; }
    public DateTime CreatedAt { get; set; }

    [StringLength(500)]
    public string Note { get; set; }

    [ForeignKey(nameof(CampaignId))]
    public virtual Campaign Campaign { get; set; }

    [ForeignKey(nameof(AgentId))]
    public virtual User Agent { get; set; }
}
