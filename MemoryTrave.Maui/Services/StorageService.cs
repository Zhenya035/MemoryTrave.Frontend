using MemoryTrave.Maui.Services.Interfaces;

namespace MemoryTrave.Maui.Services;

public class StorageService : IStorageService
{
    private const string KeyName = "EncryptedPrivateKey";
    private const string PasswordDekName = "PasswordDek";
    private const string TokenName = "JwtToken";
    
    public async Task<string?> GetTokenAsync()
    {
        try
        {
            var token = await SecureStorage.GetAsync(TokenName);
            
            return token ?? null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<bool> LoadTokenAsync(string token)
    {
        try
        {
            await SecureStorage.Default.SetAsync(TokenName, token);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public bool DeleteToken()
    {
        try
        {
            SecureStorage.Default.Remove(TokenName);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public async Task<string?> GetPassword()
    {
        try
        {
            var password = await SecureStorage.GetAsync(PasswordDekName);
            
            return password ?? null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<bool> LoadPasswordDekAsync(string password)
    {
        try
        {
            await SecureStorage.Default.SetAsync(PasswordDekName, password);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public bool DeletePassword()
    {
        try
        {
            SecureStorage.Default.Remove(PasswordDekName);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
    
    public async Task<string?> GetPrivateKey()
    {
        try
        {
            var key = await SecureStorage.GetAsync(KeyName);
            
            return key ?? null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<bool> LoadPrivateKeyAsync(string key)
    {
        try
        {
            await SecureStorage.Default.SetAsync(KeyName, key);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public bool DeletePrivateKey()
    {
        try
        {
            SecureStorage.Default.Remove(KeyName);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
}