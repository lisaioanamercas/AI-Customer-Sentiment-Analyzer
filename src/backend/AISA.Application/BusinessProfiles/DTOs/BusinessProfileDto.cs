namespace AISA.Application.BusinessProfiles.DTOs;

/// <summary>
/// DTO pentru profilul unei afaceri.
/// </summary>
public class BusinessProfileDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? Address { get; set; }
    public string SubscriptionTier { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int ReviewCount { get; set; }
}
