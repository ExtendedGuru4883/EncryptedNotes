using Blazored.LocalStorage;
using BlazorSodium.Extensions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Client;
using Client.Services.Clients;
using Client.Services.Clients.Crypto;
using Client.Services.Clients.Crypto.Interfaces;
using Client.Services.Clients.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5207/") });
builder.Services.AddScoped<IApiClient, ApiClient>();

builder.Services.AddScoped<ISignatureService, SignatureService>();
builder.Services.AddScoped<ICryptoService, CryptoService>();

builder.Services.AddBlazorSodium();
builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();