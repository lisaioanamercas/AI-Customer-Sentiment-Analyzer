using System.IdentityModel.Tokens.Jwt;
using Fluxor;
using AISA.Frontend.Services;

namespace AISA.Frontend.Store.Authentication;

/// <summary>
/// Handles all async side-effects for authentication.
/// Calls AuthService, reads JWT claims, and dispatches success/failure actions.
/// </summary>
public class AuthEffects
{
    private readonly AuthService _authService;
    private readonly IDispatcher _dispatcher;

    public AuthEffects(AuthService authService, IDispatcher dispatcher)
    {
        _authService = authService;
        _dispatcher = dispatcher;
    }

    // ── Initialise from localStorage ──────────────────────────────────────

    [EffectMethod(typeof(InitializeAuthAction))]
    public async Task HandleInitializeAuth(IDispatcher dispatcher)
    {
        var token = await _authService.GetStoredToken();
        if (!string.IsNullOrEmpty(token))
        {
            var (email, fullName, profileId) = ParseToken(token);
            dispatcher.Dispatch(new AuthInitializedAction(token, email, fullName, profileId));
        }
        else
        {
            dispatcher.Dispatch(new AuthInitializedAction(null, null, null, null));
        }
    }

    // ── Login ─────────────────────────────────────────────────────────────

    [EffectMethod]
    public async Task HandleLogin(LoginAction action, IDispatcher dispatcher)
    {
        var result = await _authService.LoginAsync(action.Email, action.Password);
        if (result.Success)
        {
            var (email, fullName, profileId) = ParseToken(result.Token);
            dispatcher.Dispatch(new LoginSuccessAction(result.Token, email ?? action.Email, fullName ?? "", profileId));
        }
        else
        {
            dispatcher.Dispatch(new LoginFailureAction(result.Error));
        }
    }

    // ── Register ──────────────────────────────────────────────────────────

    [EffectMethod]
    public async Task HandleRegister(RegisterAction action, IDispatcher dispatcher)
    {
        var result = await _authService.RegisterAsync(
            action.FullName, action.Email, action.Password, action.ConfirmPassword);

        if (result.Success)
        {
            var (email, fullName, profileId) = ParseToken(result.Token);
            dispatcher.Dispatch(new RegisterSuccessAction(result.Token, email ?? action.Email, fullName ?? action.FullName, profileId));
        }
        else
        {
            dispatcher.Dispatch(new RegisterFailureAction(result.Error));
        }
    }

    // ── Logout ────────────────────────────────────────────────────────────

    [EffectMethod(typeof(LogoutAction))]
    public async Task HandleLogout(IDispatcher dispatcher)
    {
        await _authService.LogoutAsync();
        // Reducer already resets state; no further actions needed.
    }

    // ── Helper: Parse JWT claims ──────────────────────────────────────────

    private static (string? Email, string? FullName, Guid? BusinessProfileId) ParseToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var email = jwt.Claims.FirstOrDefault(c =>
                c.Type == "email" ||
                c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

            var fullName = jwt.Claims.FirstOrDefault(c =>
                c.Type == "name" ||
                c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;

            var profileIdStr = jwt.Claims.FirstOrDefault(c => c.Type == "businessProfileId")?.Value;
            Guid? profileId = Guid.TryParse(profileIdStr, out var g) ? g : null;

            return (email, fullName, profileId);
        }
        catch
        {
            return (null, null, null);
        }
    }
}
