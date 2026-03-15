using System.Globalization;
using MemoryTrave.Maui.Services.Storage;
using MemoryTrave.Maui.ViewModel;

namespace MemoryTrave.Maui.Services.Localization;

public class LocalizationService(IStorageService service, AppShellViewModel viewModel) : ILocalizationService
{
    [Obsolete("Obsolete")]
    public async Task<bool> SetCultureAsync(string cultureCode)
    {
        var isSuccess = service.LoadCulture(cultureCode);
        
        if(isSuccess)
        {
            var culture = new CultureInfo(cultureCode);
            
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            
            return true;
        }
        return false;
    }
}