using System.Runtime.Serialization;

namespace ComtradeAssessment.Models;

[DataContract(Namespace = "http://tempuri.org/")]
public class RegisterDto
{
    [DataMember(Order = 1)]
    public string FullName { get; set; }

    [DataMember(Order = 2)]
    public string Email { get; set; }

    [DataMember(Order = 3)]
    public string Password { get; set; }

    [DataMember(Order = 4)]
    public Guid RoleId { get; set; }

    [DataMember(Order = 5)]
    public DateTime DateOfBirth { get; set; }
}
