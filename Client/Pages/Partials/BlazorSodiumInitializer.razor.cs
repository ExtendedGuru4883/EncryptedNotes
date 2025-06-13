using BlazorSodium.Services;
using Microsoft.AspNetCore.Components;

namespace Client.Pages.Partials;

public partial class BlazorSodiumInitializer : ComponentBase
{
    [Inject]
    public required IBlazorSodiumService BlazorSodiumService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await BlazorSodiumService.InitializeAsync();
    }
}