using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MemoryTrave.Maui.Services.Interfaces;
using MemoryTrave.Maui.View;

namespace MemoryTrave.Maui.ViewModel;

public partial class MapViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isAuthButVisible = true;
    
    [RelayCommand]
    private async Task OnLoginTapped()
    {
        await _navigation.GoTo(nameof(AuthPage));
    }
    
    private readonly INavigationService _navigation;
    private readonly IAuthService _authService;
    
    public MapViewModel(INavigationService navigation, IAuthService authService)
    {
        _authService = authService;
        _navigation = navigation;
        
        _authService.AuthStateChanged += OnAuthStateChanged;
        
        UpdateLoginButtonVisibility();
    }
    
    private void OnAuthStateChanged() => UpdateLoginButtonVisibility();

    private void UpdateLoginButtonVisibility()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            IsAuthButVisible = !_authService.IsAuthorized;
        });
    }
}