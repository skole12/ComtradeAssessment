using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ComtradeAssessment.Enums;
using Microsoft.EntityFrameworkCore;

namespace ComtradeAssessment.Entities;

[PrimaryKey(nameof(Id))]
public class CampaignPurchase
{
    [Required]
    public long Id { get; set; }

    [Required]
    public int CampaignId { get; set; }

    [Required]
    public int CustomerId { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public int Amount { get; set; }

    [Required]
    public int Discount { get; set; }

    [Required]
    public int AmountAfterDiscount { get; set; }

    [Required]
    public EPaymentType PaymentType { get; set; }
}
