using System.Runtime.Serialization;

namespace ComtradeAssessment.DTO;

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
    public long Id { get; set; }

    [DataMember(Order = 2)]
    public int CampaignId { get; set; }

    [DataMember(Order = 3)]
    public Guid AgentId { get; set; }

    [DataMember(Order = 4)]
    public int CustomerId { get; set; }

    [DataMember(Order = 5)]
    public DateTime CreatedAt { get; set; }
}
