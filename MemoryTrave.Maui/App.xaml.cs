using MemoryTrave.Maui.Infrastructure.Api;
using MemoryTrave.Maui.Services.Interfaces;

namespace MemoryTrave.Maui;

public partial class App : Application
{
    private readonly IServiceProvider _services;
    private readonly ApiRequestService _apiService;
    private readonly IStorageService _storageService;

    public App(IServiceProvider services, ApiRequestService apiService, IStorageService storageService)
    {
        InitializeComponent();
        _services = services;
        _apiService = apiService;
        _storageService = storageService;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        CheckToken();
        var appShell = _services.GetRequiredService<View.AppShell>();
        return new Window(appShell);
    }

    private async void CheckToken()
    {
        var token = await _storageService.GetTokenAsync();
        if (!string.IsNullOrWhiteSpace(token))
            _apiService.SetJwtToken(token);
    }
}