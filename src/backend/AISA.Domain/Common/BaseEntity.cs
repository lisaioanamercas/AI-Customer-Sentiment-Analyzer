namespace AISA.Domain.Common;

/// <summary>
/// Clasa de bază pentru toate entitățile din domeniu.
/// Oferă proprietăți comune: Id, CreatedAt, UpdatedAt.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
