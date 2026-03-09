namespace MemoryTrave.Maui.Services.Interfaces;

public interface IAuthService
{
    public bool IsAuthorized { get; }
    public event Action AuthStateChanged;
    public Task<bool> CheckAuth();
    public Task Login();
    public void Logout();
}