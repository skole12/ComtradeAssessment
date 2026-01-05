using System.ServiceModel;
using ComtradeAssessment.Models;

namespace ComtradeAssessment.Interfaces;

[ServiceContract]
public interface IUserService
{
    [OperationContract]
    Task<UserDto> RegisterUser(RegisterDto request);

    [OperationContract]
    Task<string> Login(LoginDto request);
}
