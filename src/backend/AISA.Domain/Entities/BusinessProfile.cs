using AISA.Domain.Common;
using AISA.Domain.Enums;

namespace AISA.Domain.Entities;

/// <summary>
/// Profilul unei afaceri locale (restaurant, cafenea, clinică).
/// Reprezintă clientul platformei AISA.
/// </summary>
public class BusinessProfile : BaseEntity
{
    /// <summary>Numele afacerii (ex: "Restaurant La Mama")</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Descriere scurtă a afacerii</summary>
    public string? Description { get; set; }

    /// <summary>Categoria afacerii (ex: "Restaurant", "Cafenea", "Clinică")</summary>
    public string? Category { get; set; }

    /// <summary>Adresa fizică</summary>
    public string? Address { get; set; }

    /// <summary>Tipul de abonament activ</summary>
    public SubscriptionTier SubscriptionTier { get; set; } = SubscriptionTier.Free;

    /// <summary>Colecția de recenzii asociate acestui profil</summary>
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
