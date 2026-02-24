using MemoryTrave.Maui.Api;
using MemoryTrave.Maui.Pages;

namespace MemoryTrave.Maui;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        if(string.IsNullOrEmpty(Preferences.Get("isAuthenticate", null)))
            return new Window(new LoginPage(new ApiRequestService(new HttpClient())));
        return new Window(new AppShell());
    }
}