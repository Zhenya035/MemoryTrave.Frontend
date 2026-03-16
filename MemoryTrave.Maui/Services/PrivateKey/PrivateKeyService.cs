namespace MemoryTrave.Maui.Services.PrivateKey;

public class PrivateKeyService : IPrivateKeyService
{
    private byte[]? _privateKey;
    public bool IsAuthenticated => _privateKey != null && _privateKey.Length > 0;
    
    public void SetKey(string key)
    {
        Clear(); 
        _privateKey = System.Text.Encoding.UTF8.GetBytes(key);
    }

    public string? GetKey() =>
        _privateKey != null ? System.Text.Encoding.UTF8.GetString(_privateKey) : null;

    public void Clear()
    {
        if (_privateKey == null) return;
        
        Array.Clear(_privateKey, 0, _privateKey.Length);
        _privateKey = null;
        GC.Collect();
    }
}