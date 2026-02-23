using AISA.Domain.Common;
using AISA.Domain.Enums;

namespace AISA.Domain.Entities;

/// <summary>
/// Detalii despre abonamentul unei afaceri.
/// Gestionează limitele de utilizare și perioada de valabilitate.
/// </summary>
public class Subscription : BaseEntity
{
    /// <summary>Tipul de abonament</summary>
    public SubscriptionTier Tier { get; set; } = SubscriptionTier.Free;

    /// <summary>Numărul maxim de recenzii pe lună (50 pt Free, nelimitat pt Premium)</summary>
    public int MaxReviewsPerMonth { get; set; } = 50;

    /// <summary>Numărul de recenzii utilizate în luna curentă</summary>
    public int ReviewsUsedThisMonth { get; set; } = 0;

    /// <summary>Data la care expiră abonamentul (null = Free, fără expirare)</summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>FK către profilul de business</summary>
    public Guid BusinessProfileId { get; set; }

    /// <summary>Navigare către profilul de business</summary>
    public BusinessProfile BusinessProfile { get; set; } = null!;
}
