using System.Runtime.Serialization;

namespace ComtradeAssessment.Models;

public class LoginResponseDto
{
    [DataMember(Order = 1)]
    public string AccessToken { get; set; }

    [DataMember(Order = 2)]
    public DateTime ExpiresAt { get; set; }
}
