using Blazored.SessionStorage;
using BlazorSodium.Extensions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Client;
using Client.Services;
using Client.Services.Clients;
using Client.Services.Clients.Interfaces;
using Client.Services.Interfaces;
using Microsoft.FluentUI.AspNetCore.Components;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5207/api/") });
builder.Services.AddScoped<IApiClient, ApiClient>();

builder.Services.AddScoped<IEncryptionKeyStorageService, EncryptionKeyStorageService>();
builder.Services.AddScoped<INoteService, NoteService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthStateService, AuthStateService>();
#pragma warning disable CA1416
builder.Services.AddScoped<ISignatureService, SignatureService>();
builder.Services.AddScoped<ICryptoService, CryptoService>();
#pragma warning restore CA1416

builder.Services.AddBlazorSodium();
builder.Services.AddBlazoredSessionStorage();

builder.Services.AddFluentUIComponents();


await builder.Build().RunAsync();