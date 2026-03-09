using System.Globalization;
using MemoryTrave.Maui.Services.Interfaces;
using MemoryTrave.Maui.View;
using MemoryTrave.Maui.ViewModel;

namespace MemoryTrave.Maui.Services;

public class LocalizationService(IStorageService service, AppShellViewModel viewModel) : ILocalizationService
{
    [Obsolete("Obsolete")]
    public async Task SetCultureAsync(string cultureCode)
    {
        var isSuccess = service.LoadCultureAsync(cultureCode);
        
        if(isSuccess)
        {
            var culture = new CultureInfo(cultureCode);

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            Application.Current?.MainPage?.Dispatcher.Dispatch(() =>
            {
                Application.Current.MainPage = new AppShell(viewModel);
            });
        }
    }
}