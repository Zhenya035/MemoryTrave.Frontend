using MemoryTrave.Maui.Infrastructure.Security;
using MemoryTrave.Maui.Models.Service;

namespace MemoryTrave.Maui.Services.Key;

public class KeyService : IKeyService
{
    public async Task<KeyResponse> GenerateKeys(string password)
    {
        var keys = EccP256.GenerateKeyPair();
    
        var publicKeyBytes = EccP256.PublicKeyToBytes(keys.Public);
        var publicKey = Convert.ToBase64String(publicKeyBytes);
    
        var privateKeyBytes = EccP256.PrivateKeyToBytes(keys.Private);
        var privateKey = Convert.ToBase64String(privateKeyBytes);

        var result = new KeyResponse
        {
            PrivateKey = privateKey,
            PublicKey = publicKey
        };
        
        return result;
    }
}