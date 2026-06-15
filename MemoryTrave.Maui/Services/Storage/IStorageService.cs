namespace MemoryTrave.Maui.Services.Storage;

public interface IStorageService
{
    public Task<string?> GetTokenAsync();
    public Task<bool> LoadTokenAsync(string token);
    public bool DeleteToken();
    
    public Task<string?> GetUserIdAsync();
    public Task<bool> LoadUserIdAsync(string userId);
    public bool DeleteUserId();
    
    public string GetEmail();
    public bool LoadEmail(string email);
    public bool DeleteEmail();
    
    public string GetCulture();
    public bool LoadCulture(string key);
    
    public string GetTheme();
    public bool LoadTheme(string theme);
}