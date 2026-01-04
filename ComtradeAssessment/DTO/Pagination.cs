using System.Runtime.Serialization;

namespace ComtradeAssessment.DTO;

[DataContract(Namespace = "http://tempuri.org/")]
public class Pagination
{
    [DataMember(Order = 1)]
    public int PageNumber { get; set; } = 1;

    [DataMember(Order = 2)]
    public int PageSize { get; set; } = 10;
}

[DataContract]
public class PaginationResponse
{
    [DataMember(Order = 1)]
    public int PageNumber { get; set; }

    [DataMember(Order = 2)]
    public int PageSize { get; set; }

    [DataMember(Order = 3)]
    public int TotalPages { get; set; }

    [DataMember(Order = 4)]
    public int TotalCount { get; set; }
}
