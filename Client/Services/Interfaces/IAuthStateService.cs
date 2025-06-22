namespace Client.Services.Interfaces;

public interface IAuthStateService
{
    bool IsLoggedIn { get; set; }
    
    event Action OnChange;
}