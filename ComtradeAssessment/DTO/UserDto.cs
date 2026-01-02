using System.Runtime.Serialization;

namespace ComtradeAssessment.DTO;

[DataContract]
public class UserDto
{
    [DataMember(Order = 1)]
    public Guid Id { get; set; }

    [DataMember(Order = 2)]
    public string Email { get; set; }

    [DataMember(Order = 3)]
    public string FullName { get; set; }
}
