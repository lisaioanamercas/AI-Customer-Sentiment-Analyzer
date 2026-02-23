using AISA.Domain.Entities;
using AISA.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AISA.Infrastructure.Persistence.Repositories;

public class BusinessProfileRepository : IBusinessProfileRepository
{
    private readonly AisaDbContext _context;

    public BusinessProfileRepository(AisaDbContext context)
    {
        _context = context;
    }

    public async Task<BusinessProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.BusinessProfiles
            .Include(bp => bp.Reviews)
            .FirstOrDefaultAsync(bp => bp.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<BusinessProfile>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.BusinessProfiles
            .Include(bp => bp.Reviews)
            .OrderBy(bp => bp.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<BusinessProfile> AddAsync(BusinessProfile profile, CancellationToken cancellationToken = default)
    {
        await _context.BusinessProfiles.AddAsync(profile, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return profile;
    }

    public async Task UpdateAsync(BusinessProfile profile, CancellationToken cancellationToken = default)
    {
        _context.BusinessProfiles.Update(profile);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var profile = await _context.BusinessProfiles.FindAsync(new object[] { id }, cancellationToken);
        if (profile is not null)
        {
            _context.BusinessProfiles.Remove(profile);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
