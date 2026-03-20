namespace MemoryTrave.Maui.Services.PrivateKey;

public class PrivateKeyService : IPrivateKeyService
{
    private byte[]? _privateKey;
    public bool IsAuthenticated => _privateKey != null && _privateKey.Length > 0;
    
    public void SetKey(string key)
    {
        Clear(); 
        _privateKey = Convert.FromBase64String(key);
    }

    public string? GetKey() =>
        _privateKey != null ? Convert.ToBase64String(_privateKey) : null;

    public void Clear()
    {
        if (_privateKey == null) return;
        
        Array.Clear(_privateKey, 0, _privateKey.Length);
        _privateKey = null;
        GC.Collect();
    }
}