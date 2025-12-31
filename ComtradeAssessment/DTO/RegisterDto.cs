using System.Runtime.Serialization;

namespace ComtradeAssessment.DTO
{
    [DataContract(Namespace = "http://tempuri.org/")]
    public class RegisterDto
    {
        [DataMember(Order = 1)]
        public string Name { get; set; }

        [DataMember(Order = 2)]
        public string Email { get; set; }

        [DataMember(Order = 3)]
        public string Password { get; set; }
    }
}
