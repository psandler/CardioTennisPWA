using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using CardioTennisPWA;
using CardioTennisPWA.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();

// Register query and command services independently
// Both read/write directly to localStorage (no shared state)
builder.Services.AddScoped<IGameSessionQueryService, GameSessionQueryService>();
builder.Services.AddScoped<IGameSessionCommandService, GameSessionCommandService>();

await builder.Build().RunAsync();
