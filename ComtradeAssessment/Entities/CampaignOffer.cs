using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ComtradeAssessment.Entities;

[PrimaryKey(nameof(CampaignId), nameof(CustomerId))]
public class CampaignOffer
{
    [Required]
    public int CampaignId { get; set; }

    [Required]
    public int CustomerId { get; set; }

    [Required]
    public Guid AgentId { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [DefaultValue(false)]
    public bool MadePurchase { get; set; }

    public DateTime? PurchaseDate { get; set; }

    [StringLength(500)]
    public string? Note { get; set; }

    [ForeignKey(nameof(CampaignId))]
    [DeleteBehavior(DeleteBehavior.Restrict)]
    public virtual Campaign Campaign { get; set; }

    [ForeignKey(nameof(AgentId))]
    public virtual User Agent { get; set; }
}
