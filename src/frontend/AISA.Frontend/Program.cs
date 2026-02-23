using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using AISA.Frontend;
using AISA.Frontend.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// HttpClient configurat pentru API-ul backend
var apiBaseUrl = builder.Configuration.GetValue<string>("ApiBaseUrl") ?? "https://localhost:5001";
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

// Servicii aplicație
builder.Services.AddScoped<IApiService, ApiService>();
builder.Services.AddSingleton<StateContainer>();

// MudBlazor
builder.Services.AddMudServices();

await builder.Build().RunAsync();
