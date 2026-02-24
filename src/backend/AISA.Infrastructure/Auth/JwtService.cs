using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AISA.Application.Common.Interfaces;
using AISA.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AISA.Infrastructure.Auth;

/// <summary>
/// Implementare JWT — generează token-uri semnate cu HMAC-SHA256.
/// </summary>
public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"] ?? "AisaSuperSecretKey2024!MustBe32Chars"));

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.FullName),
            new("BusinessProfileId", user.BusinessProfileId?.ToString() ?? "")
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "AISA",
            audience: _configuration["Jwt:Audience"] ?? "AISA-Frontend",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(
                int.Parse(_configuration["Jwt:ExpiryHours"] ?? "24")),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
