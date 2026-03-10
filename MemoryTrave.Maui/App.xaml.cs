using System.Globalization;
using MemoryTrave.Maui.Infrastructure.Api;
using MemoryTrave.Maui.Services.Interfaces;

namespace MemoryTrave.Maui;

public partial class App : Application
{
    private readonly IServiceProvider _services;
    private readonly ApiRequestService _apiService;
    private readonly IStorageService _storageService;
    private readonly IThemeService _themeService;

    public App(IServiceProvider services, 
        ApiRequestService apiService, 
        IStorageService storageService,
        IThemeService themeService)
    {
        InitializeComponent();
        _services = services;
        _apiService = apiService;
        _storageService = storageService;
        _themeService = themeService;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        CheckToken();
        CheckCulture();
        CheckTheme();
        
        var appShell = _services.GetRequiredService<View.AppShell>();
        return new Window(appShell);
    }

    private async void CheckToken()
    {
        var token = await _storageService.GetTokenAsync();
        if (!string.IsNullOrWhiteSpace(token))
            _apiService.SetJwtToken(token);
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