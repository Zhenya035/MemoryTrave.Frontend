using CommunityToolkit.Mvvm.ComponentModel;
using MemoryTrave.Maui.Services.Auth;

namespace MemoryTrave.Maui.ViewModel;

public partial class AppShellViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isAuthorized;

    private IAuthService _authService;
    
    public AppShellViewModel(IAuthService authService)
    {
        _authService = authService;
        _authService.AuthStateChanged += OnAuthStateChanged;
        
        UpdateAuthorizationState();
    }
    
    private void OnAuthStateChanged() => UpdateAuthorizationState();

    private void UpdateAuthorizationState()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            IsAuthorized = _authService.IsAuthorized;
        });
    }
}