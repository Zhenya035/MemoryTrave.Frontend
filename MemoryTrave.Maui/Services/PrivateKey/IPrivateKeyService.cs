namespace MemoryTrave.Maui.Services.PrivateKey;

public interface IPrivateKeyService
{
    public bool IsAuthenticated { get; }
    public void SetKey(string key);
    public string? GetKey();
    public void Clear();
}