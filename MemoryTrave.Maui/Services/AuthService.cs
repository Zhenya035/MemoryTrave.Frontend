using MemoryTrave.Maui.Services.Interfaces;

namespace MemoryTrave.Maui.Services;

public class AuthService : IAuthService
{
    private bool _isAuthorized;
    private const string JwtTokenKey = "JwtToken";

    public bool IsAuthorized => _isAuthorized;
    public event Action AuthStateChanged;

    public AuthService()
    {
        _ = CheckAuth();
    }
    
    public async Task<bool> CheckAuth()
    {
        try
        {
            var token = await SecureStorage.Default.GetAsync(JwtTokenKey);
            _isAuthorized = !string.IsNullOrEmpty(token);
            
            MainThread.BeginInvokeOnMainThread(() => AuthStateChanged?.Invoke());
            return _isAuthorized;
        }
        catch (Exception ex)
        {
            _isAuthorized = false;
            MainThread.BeginInvokeOnMainThread(() => AuthStateChanged?.Invoke());
            return false;
        }
    }

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