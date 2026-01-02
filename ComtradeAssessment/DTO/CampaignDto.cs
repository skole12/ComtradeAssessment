using System.Runtime.Serialization;

namespace ComtradeAssessment.DTO;

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
}
