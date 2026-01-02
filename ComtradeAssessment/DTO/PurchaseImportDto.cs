using System.Runtime.Serialization;

namespace ComtradeAssessment.DTO;

[DataContract(Namespace = "http://tempuri.org/")]
public class PurchaseImportDto
{
    [DataMember(Order = 1)]
    public string FileName { get; set; }

    [DataMember(Order = 2)]
    public string FileContentBase64 { get; set; }
}

[DataContract(Namespace = "http://tempuri.org/")]
public class PurchaseImportResponse
{
    [DataMember(Order = 1)]
    public bool Success { get; set; }

    [DataMember(Order = 2)]
    public string Message { get; set; }
}
