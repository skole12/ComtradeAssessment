using System.Runtime.Serialization;

namespace ComtradeAssessment.Models;

[DataContract]
public class UserDto
{
    [DataMember(Order = 1)]
    public Guid Id { get; set; }

    [DataMember(Order = 2)]
    public string Email { get; set; }

    [DataMember(Order = 3)]
    public string FullName { get; set; }

    [DataMember(Order = 4)]
    public DateTime DateOfBirth { get; set; }

    [DataMember(Order = 5)]
    public bool IsActive { get; set; }
}

[DataContract]
public class ActivateUserRequest
{
    [DataMember(IsRequired = true)]
    public Guid UserId { get; set; }

    [DataMember(IsRequired = true)]
    public bool IsActive { get; set; }
}
