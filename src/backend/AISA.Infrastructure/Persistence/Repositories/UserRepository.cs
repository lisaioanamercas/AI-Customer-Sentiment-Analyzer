using AISA.Domain.Entities;
using AISA.Domain.Interfaces;
using AISA.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AISA.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AisaDbContext _context;

    public UserRepository(AisaDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .Include(u => u.BusinessProfile)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.BusinessProfile)
            .FirstOrDefaultAsync(u => u.Email == email.ToLower().Trim());
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users
            .AnyAsync(u => u.Email == email.ToLower().Trim());
    }

    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
}
