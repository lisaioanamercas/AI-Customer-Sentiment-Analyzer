using System.Net.Http.Json;
using System.Net.Http.Headers;
using Microsoft.JSInterop;

namespace AISA.Frontend.Services;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;

    public AuthService(HttpClient http, IJSRuntime js)
    {
        _http = http;
        _js = js;
    }

    public async Task<AuthResult> RegisterAsync(string fullName, string email, string password, string confirmPassword)
    {
        var response = await _http.PostAsJsonAsync("api/auth/register", new
        {
            FullName = fullName,
            Email = email,
            Password = password,
            ConfirmPassword = confirmPassword
        });

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            if (result is not null)
            {
                await StoreToken(result.Token);
                SetAuthHeader(result.Token);
            }
            return new AuthResult { Success = true, Token = result?.Token ?? "" };
        }

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        return new AuthResult { Success = false, Error = error?.Error ?? "Eroare la înregistrare." };
    }

    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", new
        {
            Email = email,
            Password = password
        });

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            if (result is not null)
            {
                await StoreToken(result.Token);
                SetAuthHeader(result.Token);
            }
            return new AuthResult { Success = true, Token = result?.Token ?? "" };
        }

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        return new AuthResult { Success = false, Error = error?.Error ?? "Email sau parolă incorectă." };
    }

    public async Task LogoutAsync()
    {
        await _js.InvokeVoidAsync("localStorage.removeItem", "aisa_token");
        _http.DefaultRequestHeaders.Authorization = null;
    }

    public async Task<string?> GetStoredToken()
    {
        return await _js.InvokeAsync<string?>("localStorage.getItem", "aisa_token");
    }

    public async Task InitializeAsync()
    {
        var token = await GetStoredToken();
        if (!string.IsNullOrEmpty(token))
        {
            SetAuthHeader(token);
        }
    }

    private async Task StoreToken(string token)
    {
        await _js.InvokeVoidAsync("localStorage.setItem", "aisa_token", token);
    }

    private void SetAuthHeader(string token)
    {
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}

public class AuthResponse
{
    public string Token { get; set; } = "";
    public string Email { get; set; } = "";
    public string FullName { get; set; } = "";
    public DateTime ExpiresAt { get; set; }
    public Guid? BusinessProfileId { get; set; }
}

public class AuthResult
{
    public bool Success { get; set; }
    public string Token { get; set; } = "";
    public string Error { get; set; } = "";
}

public class ErrorResponse
{
    public string Error { get; set; } = "";
}
