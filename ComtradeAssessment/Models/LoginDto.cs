using System.Runtime.Serialization;

namespace ComtradeAssessment.Models;

[DataContract(Namespace = "http://tempuri.org/")]
public class LoginDto
{
    [DataMember(Order = 1)]
    public string Email { get; set; }

    [DataMember(Order = 2)]
    public string Password { get; set; }
}
