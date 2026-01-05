using System.Runtime.Serialization;

namespace ComtradeAssessment.Models;

[DataContract(Namespace = "http://tempuri.org/")]
public class BaseRequest
{
    [DataMember(Order = 1, IsRequired = false)]
    public List<Filter> Filters { get; set; } = new();

    [DataMember(Order = 2, IsRequired = true)]
    public Pagination Pagination { get; set; } = new();

    [DataMember(Order = 3)]
    public string[]? Includes { get; set; } = null;

    [DataMember(Order = 4)]
    public string? SortBy { get; set; }

    [DataMember(Order = 5)]
    public bool SortDescending { get; set; } = false;
}
