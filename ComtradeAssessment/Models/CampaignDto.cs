using System.Runtime.Serialization;
using ComtradeAssessment.Enums;

namespace ComtradeAssessment.Models;

[DataContract(Namespace = "http://tempuri.org/")]
public class CreateCampaignRequest
{
    [DataMember(Order = 1)]
    public string Name { get; set; }

    [DataMember(Order = 2)]
    public DateTime StartDate { get; set; }

    [DataMember(Order = 3)]
    public DateTime EndDate { get; set; }
}

[DataContract(Namespace = "http://tempuri.org/")]
public class CampaignResponseDto
{
    [DataMember(Order = 1)]
    public int Id { get; set; }

    [DataMember(Order = 2)]
    public string Name { get; set; }

    [DataMember(Order = 3)]
    public DateTime StartDate { get; set; }

    [DataMember(Order = 4)]
    public DateTime EndDate { get; set; }

    [DataMember(Order = 5)]
    public bool ResultsConcluded { get; set; }

    [DataMember(Order = 6)]
    public bool IsActive { get; set; }
}

[DataContract(Namespace = "http://tempuri.org/")]
public class CampaignDetailsResponseDto
{
    [DataMember(Order = 1)]
    public int CampaignId { get; set; }

    [DataMember(Order = 2)]
    public string Name { get; set; }

    [DataMember(Order = 3)]
    public DateTime StartDate { get; set; }

    [DataMember(Order = 4)]
    public DateTime EndDate { get; set; }

    [DataMember(Order = 5)]
    public bool ResultsConcluded { get; set; }

    [DataMember(Order = 6)]
    public bool IsActive { get; set; }

    [DataMember(Order = 7)]
    public int DiscountsOffered { get; set; }

    [DataMember(Order = 8)]
    public int PurchasesMade { get; set; }

    [DataMember(Order = 9)]
    public float SuccessRate { get; set; }
}

[DataContract(Namespace = "http://tempuri.org/")]
public class UpdateCampaignRequest
{
    [DataMember(Order = 1)]
    public int Id { get; set; }

    [DataMember(Order = 2)]
    public string Name { get; set; }

    [DataMember(Order = 3)]
    public DateTime StartDate { get; set; }

    [DataMember(Order = 4)]
    public DateTime EndDate { get; set; }

    [DataMember(Order = 5)]
    public bool IsActive { get; set; }
}

[DataContract]
public class GetAllCampaignsResponse
{
    [DataMember(Order = 1)]
    public List<CampaignResponseDto> Items { get; set; }

    [DataMember(Order = 2)]
    public PaginationResponse Pagination { get; set; }
}

[DataContract(Namespace = "http://tempuri.org/")]
public class CampaignJobResponseDto
{
    [DataMember(Order = 1)]
    public Guid Id { get; set; }

    [DataMember(Order = 2)]
    public JobState State { get; set; }

    [DataMember(Order = 3)]
    public string? Error { get; set; }

    [DataMember(Order = 4)]
    public DateTime CreatedAt { get; set; }

    [DataMember(Order = 5)]
    public DateTime? FinishedAt { get; set; }
}
