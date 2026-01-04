using System.ServiceModel;
using ComtradeAssessment.DTO;

namespace ComtradeAssessment.Interfaces;

[ServiceContract]
public interface IUserService
{
    [OperationContract]
    Task<UserDto> RegisterUser(RegisterDto request);

    [OperationContract]
    Task<string> Login(LoginDto request);
}
