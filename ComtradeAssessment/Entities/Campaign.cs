using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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

    [DefaultValue(false)]
    public bool ResultsConcluded { get; set; }

    [StringLength(200)]
    public string? CsvResultsPath { get; set; }

    public ICollection<CampaignOffer> CampaignOffers { get; set; }
}
