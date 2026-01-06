using System.ServiceModel;
using ComtradeAssessment.Attributes;
using ComtradeAssessment.Constants;
using ComtradeAssessment.Models;

namespace ComtradeAssessment.Interfaces;

[ServiceContract]
public interface IUserService
{
    [AuthorizeByRole(ERole.SuperAdmin)]
    [OperationContract]
    Task<UserDto> RegisterUser(RegisterDto request);

    [AllowAnonymous]
    [OperationContract]
    Task<string> Login(LoginDto request);

    [AuthorizeByRole(ERole.SuperAdmin)]
    [OperationContract]
    Task ActivateUser(ActivateUserRequest request);
}
