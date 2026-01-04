using System.Runtime.Serialization;

namespace ComtradeAssessment.DTO;

[DataContract(Namespace = "http://tempuri.org/")]
public class PagedResult<T>
{
    [DataMember(Order = 1)]
    public List<T> Items { get; set; }

    [DataMember(Order = 2)]
    public PaginationResponse Pagination { get; set; }
}
