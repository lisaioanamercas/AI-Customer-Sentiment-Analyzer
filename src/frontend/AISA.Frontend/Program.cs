using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using AISA.Frontend;
using AISA.Frontend.Services;
using Fluxor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// HttpClient configurat pentru API-ul backend
var apiBaseUrl = builder.Configuration.GetValue<string>("ApiBaseUrl") ?? "http://localhost:5000";
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

// Autentificare
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddAuthorizationCore();

// Servicii aplicație
builder.Services.AddScoped<IApiService, ApiService>();

builder.Services.AddFluxor(options => options
    .ScanAssemblies(typeof(Program).Assembly)
    .UseRouting());

// MudBlazor
builder.Services.AddMudServices();
builder.Services.AddLocalization();

var host = builder.Build();

// Inițializează culture din localStorage
var js = host.Services.GetRequiredService<IJSRuntime>();
var cultureResult = await js.InvokeAsync<string>("localStorage.getItem", "aisa_culture");
var culture = cultureResult ?? "en-US";

var cultureInfo = new System.Globalization.CultureInfo(culture);
System.Globalization.CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// Inițializează auth (restabilește token-ul din localStorage)
var authService = host.Services.GetRequiredService<AuthService>();
await authService.InitializeAsync();

await host.RunAsync();
