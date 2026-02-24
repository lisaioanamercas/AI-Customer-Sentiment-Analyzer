using AISA.Domain.Entities;

namespace AISA.Domain.Interfaces;

/// <summary>
/// Repository pentru operații cu utilizatorii.
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
    Task<User> CreateAsync(User user);
    Task UpdateAsync(User user);
}
