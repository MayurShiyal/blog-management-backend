using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Be.BlogManagementAssignment.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Be.BlogManagementAssignment.Infrastructure.Implementations.Utilities;

public sealed class JwtHelper
{
    private readonly IConfiguration _configuration;

    public JwtHelper(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
        var jwtSection = _configuration.GetSection("Jwt");

        var secretKey = jwtSection["SecretKey"]
            ?? throw new InvalidOperationException("JWT SecretKey is not configured in appsettings.json.");

        var issuer     = jwtSection["Issuer"]   ?? "BlogManagementApi";
        var audience   = jwtSection["Audience"] ?? "BlogManagementClient";
        var expiryMins = int.TryParse(jwtSection["ExpiryMinutes"], out var m) ? m : 60;

        var signingKey  = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("UserId", user.Id.ToString()),
            new Claim("Email",  user.Email),
            new Claim("Role",   user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer:             issuer,
            audience:           audience,
            claims:             claims,
            notBefore:          DateTime.UtcNow,
            expires:            DateTime.UtcNow.AddMinutes(expiryMins),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
