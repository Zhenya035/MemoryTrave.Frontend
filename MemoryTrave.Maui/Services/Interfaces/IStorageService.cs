namespace MemoryTrave.Maui.Services.Interfaces;

public interface IStorageService
{
    public Task<string?> GetTokenAsync();
    public Task<bool> LoadTokenAsync(string token);
    public bool DeleteToken();
    
    public Task<string?> GetPassword();
    public Task<bool> LoadPasswordDekAsync(string password);
    public bool DeletePassword();

    public Task<string?> GetPrivateKey();
    public Task<bool> LoadPrivateKeyAsync(string key);
    public bool DeletePrivateKey();
}