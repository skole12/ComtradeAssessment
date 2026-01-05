using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.ServiceModel;
using System.Text;
using ComtradeAssessment.Attributes;
using ComtradeAssessment.Constants;
using ComtradeAssessment.Entities;
using ComtradeAssessment.Extensions;
using ComtradeAssessment.Interfaces;
using ComtradeAssessment.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ComtradeAssessment.Services;

public class UserService(IDatabaseContext databaseContext, IOptions<JwtSettings> jwtOptions)
    : IUserService
{
    private readonly IDatabaseContext databaseContext = databaseContext;
    private readonly JwtSettings jwtSettings = jwtOptions.Value;

    /// <summary>
    /// Authenticates a user and returns access token.
    /// </summary>
    /// <param name="request">The login request containing user credentials.</param>
    /// <returns>A string representing the authentication token or session ID.</returns>
    [AllowAnonymous]
    public async Task<string> Login(LoginDto request)
    {
        var user = await databaseContext
            .Users.Where(u => u.Email == request.Email)
            .Include(u => u.Role)
            .FirstOrDefaultAsync();

        if (
            user == null
            || !user.IsActive
            || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password)
        )
        {
            throw new Exception("Invalid credentials");
        }
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(jwtSettings.Key);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
                [
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(ClaimTypes.Email, user.Email),
                    new(ClaimTypes.Role, user.Role.Name),
                ]
            ),
            Expires = DateTime.UtcNow.AddMinutes(jwtSettings.ExpiryMinutes),
            Issuer = jwtSettings.Issuer,
            Audience = jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            ),
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);
        return tokenString;
    }

    /// <summary>
    /// Registers a new user in the system.
    /// </summary>
    /// <param name="request">The registration details of the new user.</param>
    /// <returns>The created <see cref="UserDto"/> representing the new user.</returns>
    [AuthorizeByRole(ERole.SuperAdmin)]
    public async Task<UserDto> RegisterUser(RegisterDto request)
    {
        bool existingUser = await databaseContext
            .Users.Where(u => u.Email == request.Email)
            .AnyAsync();
        if (existingUser)
        {
            throw new FaultException("User already exists");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            Email = request.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            RoleId = request.RoleId,
            DateOfBirth = request.DateOfBirth,
            IsActive = true,
        };

        databaseContext.Users.Add(user);
        await databaseContext.SaveChangesAsync();

        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            DateOfBirth = user.DateOfBirth,
            IsActive = user.IsActive,
        };
    }

    /// <summary>
    /// Activates or deactivates a user based on the provided request.
    /// </summary>
    /// <param name="request">The request containing the user ID and desired active state.</param>
    [AuthorizeByRole(ERole.SuperAdmin)]
    public async Task ActivateUser(ActivateUserRequest request)
    {
        var updatedRows = await databaseContext
            .Users.Where(u => u.Id == request.UserId)
            .ExecuteUpdateAsync(u => u.SetProperty(u => u.IsActive, request.IsActive));
        if (updatedRows == 0)
            throw new FaultException("User not found");
    }
}
