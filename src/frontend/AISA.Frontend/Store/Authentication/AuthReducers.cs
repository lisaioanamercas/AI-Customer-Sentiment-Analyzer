using Fluxor;

namespace AISA.Frontend.Store.Authentication;

/// <summary>
/// Pure functions that take the current AuthState + an action and return a new AuthState.
/// NO side-effects here — only data transformations.
/// </summary>
public static class AuthReducers
{
    // ── Login ─────────────────────────────────────────────────────────────

    [ReducerMethod(typeof(LoginAction))]
    public static AuthState OnLoginStarted(AuthState state) =>
        state with { IsLoading = true, ErrorMessage = null };

    [ReducerMethod]
    public static AuthState OnLoginSuccess(AuthState state, LoginSuccessAction action) =>
        state with
        {
            IsLoading = false,
            IsAuthenticated = true,
            Token = action.Token,
            Email = action.Email,
            FullName = action.FullName,
            BusinessProfileId = action.BusinessProfileId,
            ErrorMessage = null
        };

    [ReducerMethod]
    public static AuthState OnLoginFailure(AuthState state, LoginFailureAction action) =>
        state with { IsLoading = false, IsAuthenticated = false, ErrorMessage = action.ErrorMessage };

    // ── Register ──────────────────────────────────────────────────────────

    [ReducerMethod(typeof(RegisterAction))]
    public static AuthState OnRegisterStarted(AuthState state) =>
        state with { IsLoading = true, ErrorMessage = null };

    [ReducerMethod]
    public static AuthState OnRegisterSuccess(AuthState state, RegisterSuccessAction action) =>
        state with
        {
            IsLoading = false,
            IsAuthenticated = true,
            Token = action.Token,
            Email = action.Email,
            FullName = action.FullName,
            BusinessProfileId = action.BusinessProfileId,
            ErrorMessage = null
        };

    [ReducerMethod]
    public static AuthState OnRegisterFailure(AuthState state, RegisterFailureAction action) =>
        state with { IsLoading = false, IsAuthenticated = false, ErrorMessage = action.ErrorMessage };

    // ── Logout ────────────────────────────────────────────────────────────

    [ReducerMethod(typeof(LogoutAction))]
    public static AuthState OnLogout(AuthState state) =>
        new(); // Reset to initial state

    // ── Initialisation ────────────────────────────────────────────────────

    [ReducerMethod]
    public static AuthState OnAuthInitialized(AuthState state, AuthInitializedAction action) =>
        action.Token is not null
            ? state with
            {
                IsAuthenticated = true,
                Token = action.Token,
                Email = action.Email,
                FullName = action.FullName,
                BusinessProfileId = action.BusinessProfileId
            }
            : state; // No token in storage — stay as default (unauthenticated)
}
