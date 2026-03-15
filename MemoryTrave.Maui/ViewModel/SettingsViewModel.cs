using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MemoryTrave.Maui.Resources.Localization;
using MemoryTrave.Maui.Services.Auth;
using MemoryTrave.Maui.Services.Localization;
using MemoryTrave.Maui.Services.Storage;
using MemoryTrave.Maui.Services.Theme;
using MemoryTrave.Maui.View;

namespace MemoryTrave.Maui.ViewModel;

public partial class SettingsViewModel(
    ILocalizationService localizationService,
    IStorageService storageService,
    IAuthService authService,
    IThemeService themeService,
    AppShellViewModel appShellViewModel) : ObservableObject
{
    public List<string> Languages { get; } = ["en", "ru"];
    public List<string> Themes { get; } = [Localization.ThemeSystem, Localization.ThemeLight, Localization.ThemeDark];
    
    [ObservableProperty] 
    private string _selectedLanguage = "en";
    
    [ObservableProperty] 
    private string _selectedTheme =Localization.ThemeSystem;
    
    [RelayCommand]
    [Obsolete("Obsolete")]
    private async Task ChangeLanguageAsync()
    {
        var result = await localizationService.SetCultureAsync(SelectedLanguage);
        if (result)
        {
            Application.Current?.MainPage?.Dispatcher.Dispatch(() =>
            {
                Application.Current.MainPage = new AppShell(appShellViewModel);
            });
        }
    }
    
    [RelayCommand]
    [Obsolete("Obsolete")]
    private async Task ChangeThemeAsync()
    {
        storageService.LoadTheme(SelectedTheme);
        themeService.SetThemeAsync(SelectedTheme);
    }

    [RelayCommand]
    private void LogOut()
    {
        storageService.DeletePassword();
        storageService.DeletePrivateKey();
        storageService.DeleteToken();
        authService.Logout();
    }
}