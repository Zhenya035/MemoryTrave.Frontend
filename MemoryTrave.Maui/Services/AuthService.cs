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
        CheckAuth().Wait();
    }
    
    public async Task<bool> CheckAuth()
    {
        try
        {
            var token = await SecureStorage.Default.GetAsync(JwtTokenKey);
            _isAuthorized = !string.IsNullOrEmpty(token);
            
            AuthStateChanged?.Invoke();
            return _isAuthorized;
        }
        catch (Exception ex)
        {
            _isAuthorized = false;
            AuthStateChanged?.Invoke();
            return false;
        }
    }

    public async Task Login(string jwtToken)
    {
        if (string.IsNullOrEmpty(jwtToken))
            throw new ArgumentException("Token cannot be null or empty", nameof(jwtToken));

        await SecureStorage.Default.SetAsync(JwtTokenKey, jwtToken);
        _isAuthorized = true;
        AuthStateChanged?.Invoke();
    }

    public void Logout()
    {
        SecureStorage.Default.Remove(JwtTokenKey);
        _isAuthorized = false;
        AuthStateChanged?.Invoke();
    }
}