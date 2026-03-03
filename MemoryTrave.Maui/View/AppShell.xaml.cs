using MemoryTrave.Maui.Services.Interfaces;
using MemoryTrave.Maui.ViewModel;

namespace MemoryTrave.Maui.View;

public partial class AppShell : Shell
{
    private readonly IAuthService _authService;
    
    public AppShell(IAuthService authService, AppShellViewModel vm)
    {
        InitializeComponent();
        
        _authService = authService;
        BindingContext = vm;
        
        SetTabBarIsVisible(this, false);

        _authService.AuthStateChanged += OnAuthStateChanged;
        
        Routing.RegisterRoute(nameof(AuthPage), typeof(AuthPage));
    }
    
    private async void OnAuthStateChanged()
    {
        if (_authService.IsAuthorized)
        {
            await ShowTabBar();
        }
    }
    
    private async Task ShowTabBar()
    {
        SetTabBarIsVisible(this, true);
        
        if (Current.CurrentPage != Current.CurrentPage.Navigation.NavigationStack.FirstOrDefault())
        {
            await Current.Navigation.PopModalAsync();
        }
    }
    
    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        
        if (Handler == null)
            _authService.AuthStateChanged -= OnAuthStateChanged;
    }
}