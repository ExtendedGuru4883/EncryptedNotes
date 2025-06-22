using Microsoft.AspNetCore.Components;
using Client.Models.Forms;
using Client.Services.Interfaces;

namespace Client.Pages;

public partial class Signup : ComponentBase
{
    [Inject] public required IAuthService AuthService { get; set; }
    [Inject] public required NavigationManager NavigationManager { get; set; }

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
            var serviceResponse = await AuthService.SignupAsync(_model.Username, _model.Password);
            if (serviceResponse.IsSuccess)
            {
                NavigationManager.NavigateTo("login");
                return;
            }

            _errors.Add(string.IsNullOrEmpty(serviceResponse.ErrorMessage)
                ? "Unexpected error during signup"
                : serviceResponse.ErrorMessage);
        }
        finally
        {
            _isLoading = false;
        }
    }
}