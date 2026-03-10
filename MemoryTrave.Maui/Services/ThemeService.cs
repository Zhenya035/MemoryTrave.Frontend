using MemoryTrave.Maui.Resources.Localization;
using MemoryTrave.Maui.Services.Interfaces;

namespace MemoryTrave.Maui.Services;

public class ThemeService : IThemeService
{
    public void SetThemeAsync(string mode)
    {
        if (mode == Localization.ThemeLight || mode == "1")
            Application.Current?.UserAppTheme = AppTheme.Light;
        else if (mode == Localization.ThemeDark || mode == "2")
            Application.Current?.UserAppTheme = AppTheme.Dark;
        else
            Application.Current?.UserAppTheme = AppTheme.Unspecified;
    }
}