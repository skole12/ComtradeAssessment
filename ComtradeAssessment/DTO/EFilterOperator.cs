using System.Runtime.Serialization;

namespace ComtradeAssessment.DTO;

[DataContract(Namespace = "http://tempuri.org/")]
public enum FilterOperator
{
    [EnumMember]
    Equals,

    [EnumMember]
    NotEquals,

    [EnumMember]
    Contains,

    [EnumMember]
    GreaterThan,

    [EnumMember]
    GreaterOrEqual,

    [EnumMember]
    LessThan,

    [EnumMember]
    LessOrEqual,
}

[DataContract(Namespace = "http://tempuri.org/")]
public class Filter
{
    [DataMember(Order = 1)]
    public string Field { get; set; } = null!;

    [DataMember(Order = 2)]
    public FilterOperator Operator { get; set; }

    [DataMember(Order = 3)]
    public string Value { get; set; } = null!;
}
