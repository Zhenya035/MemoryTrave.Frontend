using MemoryTrave.Maui.Services.Interfaces;

namespace MemoryTrave.Maui.View;

public partial class SettingsPage : ContentPage
{
    private readonly ILocalizationService _localizationService;
    private readonly IStorageService _storageService;
    private readonly IAuthService _authService;

    public SettingsPage(ILocalizationService localizationService, IStorageService service, IAuthService authService)
    {
        InitializeComponent();
        _localizationService = localizationService;
        _storageService = service;
        _authService = authService;

        var culture = _localizationService.CurrentCulture.Name;
        LanguagePicker.SelectedIndex = culture switch
        {
            "en" => 0,
            "ru" => 1,
            _ => 0
        };
    }
    
    private async void OnApplyLanguageClicked(object sender, EventArgs e)
    {
        var culture = LanguagePicker.SelectedIndex switch
        {
            0 => "en",
            1 => "ru",
            _ => "en"
        };
        
        await _localizationService.SetCultureAsync(culture);

        await DisplayAlertAsync("Done", "Language changed", "OK");
    }

    private void LogOut(object? sender, EventArgs e)
    {
        _storageService.DeletePassword();
        _storageService.DeletePrivateKey();
        _storageService.DeleteToken();
        _authService.Logout();
    }
}