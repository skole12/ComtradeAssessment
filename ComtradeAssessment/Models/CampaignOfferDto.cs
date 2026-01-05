using System.Runtime.Serialization;

namespace ComtradeAssessment.Models;

[DataContract(Namespace = "http://tempuri.org/")]
public class CreateCampaignOfferRequest
{
    [DataMember(Order = 1)]
    public int CampaignId { get; set; }

    [DataMember(Order = 2)]
    public int CustomerId { get; set; }
}

[DataContract(Namespace = "http://tempuri.org/")]
public class CampaignOfferResponseDto
{
    [DataMember(Order = 1)]
    public int CampaignId { get; set; }

    [DataMember(Order = 2)]
    public Guid AgentId { get; set; }

    [DataMember(Order = 3)]
    public int CustomerId { get; set; }

    [DataMember(Order = 4)]
    public DateTime CreatedAt { get; set; }

    [DataMember(Order = 5)]
    public bool MadePurchase { get; set; }

    [DataMember(Order = 6)]
    public DateTime? PurchaseDate { get; set; }

    [DataMember(Order = 7)]
    public string? Note { get; set; }
}

[DataContract(Namespace = "http://tempuri.org/")]
public class DeleteCampaignOfferRequest
{
    [DataMember(Order = 1)]
    public int CampaignId { get; set; }

    [DataMember(Order = 2)]
    public int CustomerId { get; set; }
}

[DataContract(Namespace = "http://tempuri.org/")]
public class GetAllCampaignOffersResponse
{
    [DataMember(Order = 1)]
    public List<CampaignOfferResponseDto> Items { get; set; }

    [DataMember(Order = 2)]
    public PaginationResponse Pagination { get; set; }
}
