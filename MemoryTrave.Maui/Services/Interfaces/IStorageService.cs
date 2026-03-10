namespace MemoryTrave.Maui.Services.Interfaces;

public interface IStorageService
{
    public Task<string?> GetTokenAsync();
    public Task<bool> LoadTokenAsync(string token);
    public bool DeleteToken();
    
    public Task<string?> GetPasswordAsync();
    public Task<bool> LoadPasswordDekAsync(string password);
    public bool DeletePassword();

    public Task<string?> GetPrivateKeyAsync();
    public Task<bool> LoadPrivateKeyAsync(string key);
    public bool DeletePrivateKey();
    
    public string GetCulture();
    public bool LoadCulture(string key);
    
    public string GetTheme();
    public bool LoadTheme(string theme);
}