using BCrypt.Net;
using Be.BlogManagementAssignment.Domain.Entities;
using Be.BlogManagementAssignment.Domain.Enums;
using Be.BlogManagementAssignment.Infrastructure.Persistence.Models;

namespace Be.BlogManagementAssignment.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAdminAsync(ApplicationDbContext context)
    {
        if (!context.Users.Any())
        {
            var admin = new User
            {
                FirstName = "Super",
                LastName = "Admin",
                Email = "admin@bms.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = UserRole.Admin,
                Status = UserStatus.Active,
                IsVerified = true
            };

            context.Users.Add(admin);

            await context.SaveChangesAsync();
        }
    }
}