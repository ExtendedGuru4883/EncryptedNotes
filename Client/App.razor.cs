using BlazorSodium.Services;
using Microsoft.AspNetCore.Components;

namespace Client;

public partial class App : ComponentBase
{
    [Inject]
    public required IBlazorSodiumService BlazorSodiumService { get; set; }
    
    private bool _isBlazorSodiumInitialized;

    protected override async Task OnInitializedAsync()
    {
        await BlazorSodiumService.InitializeAsync();
        _isBlazorSodiumInitialized = true;
    }
}