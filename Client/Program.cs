using Blazored.SessionStorage;
using BlazorSodium.Extensions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Client;
using Client.Helpers.Crypto;
using Client.Helpers.Crypto.Interfaces;
using Client.Services.Clients;
using Client.Services.Clients.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5207/api/") });
builder.Services.AddScoped<IApiClient, ApiClient>();

#pragma warning disable CA1416
builder.Services.AddScoped<ISignatureHelper, SignatureHelper>();
builder.Services.AddScoped<ICryptoHelper, CryptoHelper>();
#pragma warning restore CA1416

builder.Services.AddBlazorSodium();
builder.Services.AddBlazoredSessionStorage();

await builder.Build().RunAsync();