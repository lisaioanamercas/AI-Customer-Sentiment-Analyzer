using AISA.Domain.Entities;

namespace AISA.Domain.Interfaces;

/// <summary>
/// Repository pentru gestionarea profilurilor de afaceri.
/// </summary>
public interface IBusinessProfileRepository
{
    Task<BusinessProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BusinessProfile>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<BusinessProfile> AddAsync(BusinessProfile profile, CancellationToken cancellationToken = default);
    Task UpdateAsync(BusinessProfile profile, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
