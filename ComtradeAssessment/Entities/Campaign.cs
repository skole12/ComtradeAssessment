using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ComtradeAssessment.Entities;

[PrimaryKey(nameof(Id))]
public class Campaign
{
    public int Id { get; set; }

    [StringLength(200)]
    [Required]
    public string Name { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public bool ResultsConcluded { get; set; }

    [DefaultValue(true)]
    public bool IsActive { get; set; }

    [StringLength(200)]
    public string? CsvResultsPath { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public Guid CreatedBy { get; set; }

    [Required]
    public int DiscountsOffered { get; set; }

    [Required]
    public int PurchasesMade { get; set; }

    [ForeignKey(nameof(CreatedBy))]
    public virtual User CreatedByUser { get; set; }

    public ICollection<CampaignOffer> CampaignOffers { get; set; }
}
