using Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace Client.Layout;

public partial class TopBar : ComponentBase, IDisposable
{
    [Inject] public required IAuthStateService AuthStateService { get; set; }
    [Inject] public required NavigationManager NavigationManager { get; set; }
    [Inject] public required IAuthService AuthService { get; set; }

    protected override void OnInitialized()
    {
        AuthStateService.OnChange += StateHasChanged;
    }
    
    private void GoToLogin() => NavigationManager.NavigateTo("login");
    private void GoToSignup() => NavigationManager.NavigateTo("signup");

    private async Task Logout()
    {
        await AuthService.LogoutAsync();
        NavigationManager.NavigateTo("login");
    }

    public void Dispose()
    {
        AuthStateService.OnChange -= StateHasChanged;
    }
}