using Fluxor;

namespace AISA.Frontend.Store.Authentication;

/// <summary>
/// Single source of truth for authentication state.
/// Immutable record — reducers always produce a new instance.
/// </summary>
[FeatureState]
public record AuthState
{
    /// <summary>Whether an auth API call is in-flight.</summary>
    public bool IsLoading { get; init; } = false;

    /// <summary>True once the user has a valid JWT.</summary>
    public bool IsAuthenticated { get; init; } = false;

    /// <summary>The raw JWT token stored in memory.</summary>
    public string? Token { get; init; }

    /// <summary>Display name of the logged-in user.</summary>
    public string? FullName { get; init; }

    /// <summary>Email of the logged-in user.</summary>
    public string? Email { get; init; }

    /// <summary>The business profile ID extracted from the JWT claims.</summary>
    public Guid? BusinessProfileId { get; init; }

    /// <summary>Error message from the last failed auth operation.</summary>
    public string? ErrorMessage { get; init; }
}
