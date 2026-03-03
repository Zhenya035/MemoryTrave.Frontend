using MemoryTrave.Maui.Infrastructure.Api;
using MemoryTrave.Maui.Services;
using MemoryTrave.Maui.Services.Interfaces;
using MemoryTrave.Maui.View;
using MemoryTrave.Maui.ViewModel;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace MemoryTrave.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseSkiaSharp()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<HttpClient>();
        builder.Services.AddSingleton<ApiRequestService>();
        
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<IDialogService, DialogService>();
        builder.Services.AddSingleton<IKeyService, KeyService>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();

        builder.Services.AddSingleton<View.AppShell>();
        builder.Services.AddSingleton<AppShellViewModel>();
        
        builder.Services.AddTransient<MapPage>();
        builder.Services.AddTransient<MapViewModel>();
        
        builder.Services.AddTransient<AuthPage>();
        builder.Services.AddTransient<AuthViewModel>();
        
#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}