namespace MemoryTrave.Maui.Services.Auth;

public class AuthService : IAuthService
{
    private bool _isAuthorized;
    private const string JwtTokenKey = "JwtToken";

    public bool IsAuthorized => _isAuthorized;
    public event Action AuthStateChanged;

    public async Task Login()
    { 
        _isAuthorized = true;
        MainThread.BeginInvokeOnMainThread(() => AuthStateChanged?.Invoke());
    }

    public void Logout()
    {
        _isAuthorized = false;
        MainThread.BeginInvokeOnMainThread(() => AuthStateChanged?.Invoke());
    }
}