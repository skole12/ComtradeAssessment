using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ComtradeAssessment.DTO;
using ComtradeAssessment.Entities;
using ComtradeAssessment.Extensions;
using ComtradeAssessment.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ComtradeAssessment.Services
{
    public class UserService : IUserService
    {
        private readonly IDatabaseContext databaseContext;
        private readonly JwtSettings jwtSettings;

        public UserService(IDatabaseContext databaseContext, IOptions<JwtSettings> jwtOptions)
        {
            this.databaseContext = databaseContext;
            jwtSettings = jwtOptions.Value;
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            return await databaseContext
                .Set<User>()
                .Select(u => new UserDto { Id = u.Id, Email = u.Email })
                .ToListAsync();
        }

        public async Task<string> Login(LoginDto request)
        {
            var user = await databaseContext
                .Users.Where(u => u.Email == request.Email)
                .FirstOrDefaultAsync();

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
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

        public async Task<UserDto> RegisterUserAsync(RegisterDto request)
        {
            bool existingUser = await databaseContext
                .Users.Where(u => u.Email == request.Email)
                .AnyAsync();
            if (existingUser)
            {
                throw new Exception("User already exists");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            };

            databaseContext.Set<User>().Add(user);
            await databaseContext.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
            };
        }
    }
}
