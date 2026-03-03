using System.Globalization;
using MemoryTrave.Maui.Resources.Localization;
using MemoryTrave.Maui.Services.Interfaces;

namespace MemoryTrave.Maui.Services;

public class LocalizationService : ILocalizationService
{
    private CultureInfo _currentCulture;

    public CultureInfo CurrentCulture => _currentCulture;
    public event Action CultureChanged;

    public LocalizationService()
    {
        _currentCulture = CultureInfo.DefaultThreadCurrentCulture ?? CultureInfo.CurrentCulture;
    }

    public Task SetCultureAsync(string cultureCode)
    {
        if (string.IsNullOrEmpty(cultureCode))
            throw new ArgumentNullException(nameof(cultureCode));

        var culture = new CultureInfo(cultureCode);

        _currentCulture = culture;

        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;

        Localization.Culture = culture;

        CultureChanged?.Invoke();

        Preferences.Set("AppCulture", cultureCode);

        return Task.CompletedTask;
    }

    public async Task LoadSavedCultureAsync()
    {
        var savedCulture = Preferences.Get("AppCulture", null);
        if (!string.IsNullOrEmpty(savedCulture))
        {
            await SetCultureAsync(savedCulture);
        }
    }
}