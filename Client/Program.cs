using System.Runtime.Versioning;
using Blazored.LocalStorage;
using BlazorSodium.Extensions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Client;
using Client.Services.Clients;
using Client.Services.Clients.Interfaces;
using Client.Services.Crypto;
using Client.Services.Crypto.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5207/") });
builder.Services.AddScoped<IApiClient, ApiClient>();

#pragma warning disable CA1416
builder.Services.AddScoped<ISignatureService, SignatureService>();
builder.Services.AddScoped<ICryptoService, CryptoService>();
#pragma warning restore CA1416

builder.Services.AddBlazorSodium();
builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();