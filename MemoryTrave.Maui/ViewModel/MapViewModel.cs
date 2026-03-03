using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MemoryTrave.Maui.Services.Interfaces;
using MemoryTrave.Maui.View;

namespace MemoryTrave.Maui.ViewModel;

public partial class MapViewModel(INavigationService navigation) : ObservableObject
{
    [RelayCommand]
    private async Task OnLoginTapped()
    {
        await navigation.GoTo(nameof(AuthPage));
    }
}