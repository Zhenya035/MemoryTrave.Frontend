using MemoryTrave.Maui.Resources.Localization;
using MemoryTrave.Maui.Services.Interfaces;

namespace MemoryTrave.Maui.Services;

public class AuthService : IAuthService
{
    private readonly IDialogService _dialogService;
    private bool _isAuthorized;
    private const string JwtTokenKey = "JwtToken";

    public bool IsAuthorized => _isAuthorized;
    public event Action AuthStateChanged;

    public AuthService(IDialogService dialogService)
    {
        _dialogService = dialogService;
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

    public async Task Login(string jwtToken)
    {
        if (string.IsNullOrEmpty(jwtToken))
        {
            _dialogService.ShowMessage(Localization.Error, Localization.JwtError);
            return;
        }

        await SecureStorage.Default.SetAsync(JwtTokenKey, jwtToken);
        _isAuthorized = true;
        MainThread.BeginInvokeOnMainThread(() => AuthStateChanged?.Invoke());
    }

    public void Logout()
    {
        SecureStorage.Default.Remove(JwtTokenKey);
        _isAuthorized = false;
        MainThread.BeginInvokeOnMainThread(() => AuthStateChanged?.Invoke());
    }
}