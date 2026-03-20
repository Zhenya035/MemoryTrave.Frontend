namespace MemoryTrave.Maui.Services.Navigation;

public class NavigationService : INavigationService
{
    public async Task GoTo(string route) =>
        await Shell.Current.GoToAsync(route);

    public async Task GoBack() =>
        await Shell.Current.GoToAsync("..");
}