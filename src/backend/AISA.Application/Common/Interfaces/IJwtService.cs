using AISA.Domain.Entities;

namespace AISA.Application.Common.Interfaces;

/// <summary>
/// Serviciu de generare și validare token-uri JWT.
/// </summary>
public interface IJwtService
{
    /// <summary>Generează un JWT pentru utilizatorul specificat.</summary>
    string GenerateToken(User user);
}
