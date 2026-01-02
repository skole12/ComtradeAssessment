using System.ServiceModel;
using ComtradeAssessment.Attributes;
using ComtradeAssessment.Constants;
using ComtradeAssessment.DTO;

namespace ComtradeAssessment.Interfaces;

[ServiceContract]
public interface IUserService
{
    [OperationContract]
    [AuthorizeByRole(ERole.SuperAdmin)]
    Task<UserDto> RegisterUser(RegisterDto request);

    [OperationContract]
    [AllowAnonymous]
    Task<string> Login(LoginDto request);
}
