namespace MemoryTrave.Maui.Services.Interfaces;

public interface INavigationService
{
    public Task GoTo(string route);
    public Task GoBack();
}