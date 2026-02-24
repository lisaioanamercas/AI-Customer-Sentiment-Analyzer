using AISA.Domain.Common;

namespace AISA.Domain.Entities;

/// <summary>
/// Utilizatorul platformei AISA.
/// Relație 1:1 cu BusinessProfile.
/// </summary>
public class User : BaseEntity
{
    /// <summary>Numele complet al utilizatorului</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>Adresa de email (unică, folosită la autentificare)</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Hash-ul parolei (BCrypt)</summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>Dacă email-ul a fost confirmat</summary>
    public bool EmailConfirmed { get; set; } = false;

    /// <summary>FK opțional către profilul de afacere</summary>
    public Guid? BusinessProfileId { get; set; }

    /// <summary>Profilul de afacere asociat</summary>
    public BusinessProfile? BusinessProfile { get; set; }
}
