using ComtradeAssessment.Constants;
using ComtradeAssessment.Entities;
using ComtradeAssessment.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ComtradeAssessment.Seeding;

public static class Seed
{
    public static async Task SeedData(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

        var existingRoles = await databaseContext.Roles.Select(r => r.Name).ToListAsync();

        var rolesToSeed = new[] { ERole.SuperAdmin, ERole.SalesManager, ERole.SalesAgent };

        var missingRoles = rolesToSeed
            .Except(existingRoles)
            .Select(r => new Role { Id = Guid.NewGuid(), Name = r })
            .ToList();

        if (missingRoles.Count != 0)
        {
            databaseContext.Roles.AddRange(missingRoles);
            await databaseContext.SaveChangesAsync();
        }

        if (!await databaseContext.Users.AnyAsync(u => u.Role.Name == ERole.SuperAdmin))
        {
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var initialPassword = config["SeedUsers:SuperAdminPassword"];

            var superAdminRoleId = await databaseContext
                .Roles.Where(r => r.Name == ERole.SuperAdmin)
                .Select(r => r.Id)
                .SingleAsync();

            databaseContext.Users.Add(
                new User
                {
                    Email = "superadmin@system",
                    IsActive = true,
                    RoleId = superAdminRoleId,
                    FullName = "SUPER ADMIN",
                    CreatedAt = DateTime.UtcNow,
                    DateOfBirth = DateTime.UtcNow,
                    Password = BCrypt.Net.BCrypt.HashPassword(initialPassword),
                }
            );

            await databaseContext.SaveChangesAsync();
        }
    }
}
