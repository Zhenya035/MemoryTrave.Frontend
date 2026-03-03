using CommunityToolkit.Mvvm.ComponentModel;
using MemoryTrave.Maui.Services.Interfaces;

namespace MemoryTrave.Maui.ViewModel;

public partial class AppShellViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isAuhorized;
    
    [ObservableProperty]
    private bool isGuest = true;

    private IAuthService _authService;
    
    public AppShellViewModel(IAuthService authService)
    {
        _authService = authService;
        authService.AuthStateChanged += OnAuthStateChanged;
        
        OnAuthStateChanged();
    }

    private void OnAuthStateChanged()
    {
        IsAuhorized = _authService.IsAuthorized;
        OnPropertyChanged(nameof(IsGuest)); 
    }
}