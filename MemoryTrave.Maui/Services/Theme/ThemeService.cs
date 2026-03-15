namespace MemoryTrave.Maui.Services.Theme;

public class ThemeService : IThemeService
{
    public void SetThemeAsync(string mode)
    {
        if (mode == Resources.Localization.Localization.ThemeLight || mode == "1")
            Application.Current?.UserAppTheme = AppTheme.Light;
        else if (mode == Resources.Localization.Localization.ThemeDark || mode == "2")
            Application.Current?.UserAppTheme = AppTheme.Dark;
        else
            Application.Current?.UserAppTheme = AppTheme.Unspecified;
    }
}