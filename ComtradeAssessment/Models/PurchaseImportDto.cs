using System.Runtime.Serialization;

namespace ComtradeAssessment.Models;

[DataContract(Namespace = "http://tempuri.org/")]
public class PurchaseImportDto
{
    [DataMember(Order = 1)]
    public int CampaignId { get; set; }

    [DataMember(Order = 2)]
    public string FileContentBase64 { get; set; }
}

[DataContract(Namespace = "http://tempuri.org/")]
public class PurchaseImportResponse
{
    [DataMember(Order = 1)]
    public Guid JobId { get; set; }

    [DataMember(Order = 2)]
    public string Message { get; set; }
}
