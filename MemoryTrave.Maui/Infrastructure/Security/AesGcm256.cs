using System.Security.Cryptography;
using System.Text;

namespace MemoryTrave.Maui.Infrastructure.Security;

public class AesGcm256
{
    private const int NonceSize = 12;
    private const int TagSize = 16;
    
    public static string Encrypt(string text, byte[] key)
    {
        using var aesGcm = new AesGcm(key);

        var nonce = RandomNumberGenerator.GetBytes(NonceSize);
        var cipherText = new byte[text.Length];
        var tag = new byte[TagSize];
        
        aesGcm.Encrypt(nonce, Encoding.UTF8.GetBytes(text), cipherText, tag);

        var combined = new byte[nonce.Length + tag.Length + cipherText.Length];
        Buffer.BlockCopy(nonce, 0, combined, 0, nonce.Length);
        Buffer.BlockCopy(tag, 0, combined, nonce.Length, tag.Length);
        Buffer.BlockCopy(cipherText, 0, combined, nonce.Length + tag.Length, cipherText.Length);
        
        return Convert.ToBase64String(combined);
    }

    public static string Decrypt(string base64String, byte[] key)
    {
        var combined = Convert.FromBase64String(base64String);
        
        var nonce = new byte[NonceSize];
        var tag = new byte[TagSize];
        var cipherText = new byte[combined.Length - NonceSize - TagSize];

        Buffer.BlockCopy(combined, 0, nonce, 0, NonceSize);
        Buffer.BlockCopy(combined, NonceSize, tag, 0, TagSize);
        Buffer.BlockCopy(combined, NonceSize + TagSize, cipherText, 0, cipherText.Length);
        
        using var aesGcm = new AesGcm(key);
        
        var plainData = new byte[cipherText.Length];
        
        aesGcm.Decrypt(nonce, cipherText, tag, plainData);
        
        return Encoding.UTF8.GetString(plainData);
    }
}