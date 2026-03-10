using MemoryTrave.Maui.ViewModel;

namespace MemoryTrave.Maui.View;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(SettingsViewModel settingsViewModel)
    {
        InitializeComponent();
        BindingContext = settingsViewModel;
    }
}