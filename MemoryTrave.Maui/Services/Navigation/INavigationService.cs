namespace MemoryTrave.Maui.Services.Navigation;

public interface INavigationService
{
    public Task GoTo(string route);
    public Task GoBack();
}