using Microsoft.AspNetCore.Components;
using Client.Models.Forms;
using Client.Services.Interfaces;

namespace Client.Pages;

public partial class Login : ComponentBase
{
    [Inject] public required NavigationManager NavigationManager { get; set; }
    [Inject] public required IAuthService AuthService { get; set; }

    private readonly AuthFormModel _model = new();
    private readonly List<string> _errors = [];
    private bool _isLoading;
    
    protected override async Task OnInitializedAsync()
    {
        await AuthService.LogoutAsync();
    }

    private async Task SubmitAsync()
    {
        _errors.Clear();
        _isLoading = true;
        await Task.Yield();

        try
        {
            var serviceResponse = await AuthService.LoginAsync(_model.Username, _model.Password);
            if (serviceResponse.IsSuccess)
            {
                NavigationManager.NavigateTo("/notes");
                return;
            }

            _errors.Add(string.IsNullOrEmpty(serviceResponse.ErrorMessage)
                ? "Unexpected error during login"
                : serviceResponse.ErrorMessage);
        }
        finally
        {
            _isLoading = false;
        }
    }
}