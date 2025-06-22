using Client.Services.Interfaces;

namespace Client.Services;

public class AuthStateService : IAuthStateService
{
    private bool _isLoggedIn = true;
    public bool IsLoggedIn
    {
        get =>  _isLoggedIn;
        set
        {
            if (value == _isLoggedIn) return;
            _isLoggedIn = value;
            NotifyStateChanged();
        }
    }

    public event Action? OnChange;
    
    private void NotifyStateChanged() => OnChange?.Invoke();
}