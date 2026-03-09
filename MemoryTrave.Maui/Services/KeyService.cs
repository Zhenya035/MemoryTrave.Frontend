using MemoryTrave.Maui.Infrastructure.Api;
using MemoryTrave.Maui.Infrastructure.Security;
using MemoryTrave.Maui.Models.Authorization;
using MemoryTrave.Maui.Models.Service;
using MemoryTrave.Maui.Services.Interfaces;

namespace MemoryTrave.Maui.Services;

public class KeyService(ApiRequestService apiService) : IKeyService
{
    public async Task<KeyResponse> GenerateKeys(string password)
    {
        var keys = EccP256.GenerateKeyPair();
    
        var publicKeyBytes = EccP256.PublicKeyToBytes(keys.Public);
        var publicKey = Convert.ToBase64String(publicKeyBytes);
    
        var privateKeyBytes = EccP256.PrivateKeyToBytes(keys.Private);
        var  privateKey = Convert.ToBase64String(privateKeyBytes);

        var pbkdf2Data = Pbkdf2.DeriveKey(password);
    
        var encryptedPrivateKey = AesGcm256.Encrypt(privateKey, pbkdf2Data.Key);
        var encryptedBytes = Convert.FromBase64String(encryptedPrivateKey);
    
        var combinedPrivateKey = new byte[pbkdf2Data.Salt.Length + encryptedBytes.Length];
        Buffer.BlockCopy(pbkdf2Data.Salt, 0, combinedPrivateKey, 0, pbkdf2Data.Salt.Length);
        Buffer.BlockCopy(encryptedBytes, 0, combinedPrivateKey, pbkdf2Data.Salt.Length,encryptedBytes.Length);
    
        encryptedPrivateKey = Convert.ToBase64String(combinedPrivateKey);

        var body = new AddKeyRequest
        {
            PublicKey = publicKey,
            EncryptedPrivateKey = encryptedPrivateKey
        };
    
        var response = await apiService.PutRequest(URL.AddKeys(), body);

        if (response.IsSuccess)
        {
            var result = new KeyResponse
            {
                IsSuccess = true,
                EncryptedPasswordKey = Convert.ToBase64String(pbkdf2Data.Key),
                EncryptedPrivateKey = encryptedPrivateKey
            };
            return result;
        }
        else
        {
            var result = new KeyResponse
            {
                IsSuccess = false,
                Error =  response.ErrorMessage
            };
            return result;
        }
    }
}