using System.Runtime.Serialization;

namespace ComtradeAssessment.DTO
{
    [DataContract(Namespace = "http://tempuri.org/")]
    public class LoginDto
    {
        [DataMember(Order = 1)]
        public string Email { get; set; }

        [DataMember(Order = 2)]
        public string Password { get; set; }
    }
}
