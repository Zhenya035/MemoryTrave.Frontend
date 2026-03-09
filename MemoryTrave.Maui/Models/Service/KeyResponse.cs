namespace MemoryTrave.Maui.Models.Service;

public class KeyResponse
{
    public bool IsSuccess { get; set; }
    public string? EncryptedPasswordKey { get; set; } = null;
    public string? EncryptedPrivateKey { get; set; } =  null;
    public string? Error { get; set; } = null;
}