using System.ComponentModel.DataAnnotations;
using ComtradeAssessment.Enums;
using Microsoft.EntityFrameworkCore;

namespace ComtradeAssessment.Entities;

[PrimaryKey(nameof(Id))]
public class Purchase
{
    [Required]
    public long Id { get; set; }

    [Required]
    public long? CampaignOfferId { get; set; }

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

    public EPaymentType PaymentType { get; set; }
}
