namespace MemoryTrave.Maui.Response;

public class AddKeyRequest
{
    public string PublicKey { get; set; } = string.Empty;
    public string EncryptedPrivateKey { get; set; } = string.Empty;
}