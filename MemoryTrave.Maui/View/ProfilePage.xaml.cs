using MemoryTrave.Maui.Services.Interfaces;

namespace MemoryTrave.Maui.View;

public partial class ProfilePage : ContentPage
{
    private readonly IAuthService authService;
    
    public ProfilePage(IAuthService  authService)
    {
        InitializeComponent();
        this.authService = authService;
    }

    private void Logout(object? sender, EventArgs e)
    {
        authService.Logout();
    }
}