namespace AISA.Frontend.Store.Authentication;

// ── Login ────────────────────────────────────────────────────────────────────

/// <summary>Dispatch this to begin a login flow.</summary>
public record LoginAction(string Email, string Password);

/// <summary>Dispatched by AuthEffects when login succeeds.</summary>
public record LoginSuccessAction(string Token, string Email, string FullName, Guid? BusinessProfileId);

/// <summary>Dispatched by AuthEffects when login fails.</summary>
public record LoginFailureAction(string ErrorMessage);

// ── Register ─────────────────────────────────────────────────────────────────

/// <summary>Dispatch this to begin a registration flow.</summary>
public record RegisterAction(string FullName, string Email, string Password, string ConfirmPassword);

/// <summary>Dispatched by AuthEffects when registration succeeds.</summary>
public record RegisterSuccessAction(string Token, string Email, string FullName, Guid? BusinessProfileId);

/// <summary>Dispatched by AuthEffects when registration fails.</summary>
public record RegisterFailureAction(string ErrorMessage);

// ── Logout ───────────────────────────────────────────────────────────────────

/// <summary>Dispatch this to log the user out and clear the store.</summary>
public record LogoutAction;

// ── Initialisation ───────────────────────────────────────────────────────────

/// <summary>Dispatch on app startup to restore auth state from localStorage.</summary>
public record InitializeAuthAction;

/// <summary>Dispatched by AuthEffects after restoring state from localStorage.</summary>
public record AuthInitializedAction(string? Token, string? Email, string? FullName, Guid? BusinessProfileId);
