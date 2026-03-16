using System.Globalization;
using MemoryTrave.Maui.Services.Auth;
using MemoryTrave.Maui.Services.PrivateKey;
using MemoryTrave.Maui.Services.Storage;
using MemoryTrave.Maui.Services.Theme;

namespace MemoryTrave.Maui;

public partial class App : Application
{
    private readonly IServiceProvider _services;
    private readonly IStorageService _storageService;
    private readonly IThemeService _themeService;
    private readonly IPrivateKeyService _privateKeyService;
    private readonly IAuthService _authService;

    public App(IServiceProvider services,
        IStorageService storageService,
        IThemeService themeService, 
        IPrivateKeyService privateKeyService, 
        IAuthService authService)
    {
        InitializeComponent();
        _services = services;
        _storageService = storageService;
        _themeService = themeService;
        _privateKeyService = privateKeyService;
        _authService = authService;
    }

    protected override void OnSleep()
    {
        base.OnSleep();
        
        _privateKeyService.Clear();
        _storageService.DeleteToken();
        _authService.Logout();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        CheckCulture();
        CheckTheme();
        
        var appShell = _services.GetRequiredService<View.AppShell>();
        return new Window(appShell);
    }
    
    private void CheckCulture()
    {
        var cultureCode = _storageService.GetCulture();
        
        if(cultureCode != string.Empty)
        {
            var culture = new CultureInfo(cultureCode);
        
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
        else
        {
            var culture = new CultureInfo("en");
        
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
    }

    private void CheckTheme()
    {
        var themeCode = _storageService.GetTheme();
        _themeService.SetThemeAsync(themeCode);
    }
}