namespace MemoryTrave.Maui.Services.Storage;

public class StorageService : IStorageService
{
    private const string KeyName = "EncryptedPrivateKey";
    private const string PasswordDekName = "PasswordDek";
    private const string TokenName = "JwtToken";
    private const string CultureName = "Culture";
    private const string ThemeName = "Theme";
    
    public async Task<string?> GetTokenAsync()
    {
        try
        {
            var token = await SecureStorage.Default.GetAsync(TokenName);
            
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

    public async Task<string?> GetPasswordAsync()
    {
        try
        {
            var password = await SecureStorage.Default.GetAsync(PasswordDekName);
            
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
    
    public async Task<string?> GetPrivateKeyAsync()
    {
        try
        {
            var key = await SecureStorage.Default.GetAsync(KeyName);
            
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

    public string GetCulture()
    {
        try
        {
            var culture = Preferences.Default.Get(CultureName, string.Empty);
            
            return culture;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return string.Empty;
        }
    }

    public bool LoadCulture(string culture)
    {
        try
        {
            Preferences.Default.Set(CultureName, culture);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
    
    public string GetTheme()
    {
        try
        {
            var theme = Preferences.Default.Get(ThemeName, string.Empty);
            
            return theme;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return string.Empty;
        }
    }

    public bool LoadTheme(string theme)
    {
        try
        {
            if(theme == Resources.Localization.Localization.ThemeLight)
                Preferences.Default.Set(ThemeName, "1");
            else if(theme == Resources.Localization.Localization.ThemeDark)
                Preferences.Default.Set(ThemeName, "2");
            else
                Preferences.Default.Set(ThemeName, "0");
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
}