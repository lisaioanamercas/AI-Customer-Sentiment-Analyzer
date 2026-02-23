using AISA.Frontend.Models;

namespace AISA.Frontend.Services;

/// <summary>
/// Simple State Store — menține starea globală a aplicației.
/// Notifică componentele abonate la schimbări prin evenimentul OnChange.
/// </summary>
public class StateContainer
{
    private BusinessProfileModel? _currentProfile;
    private List<ReviewModel> _reviews = new();

    public BusinessProfileModel? CurrentProfile
    {
        get => _currentProfile;
        set
        {
            _currentProfile = value;
            NotifyStateChanged();
        }
    }

    public List<ReviewModel> Reviews
    {
        get => _reviews;
        set
        {
            _reviews = value;
            NotifyStateChanged();
        }
    }

    public Guid? ActiveBusinessProfileId => CurrentProfile?.Id;

    /// <summary>Eveniment declanșat la orice schimbare de stare.</summary>
    public event Action? OnChange;

    private void NotifyStateChanged() => OnChange?.Invoke();
}
