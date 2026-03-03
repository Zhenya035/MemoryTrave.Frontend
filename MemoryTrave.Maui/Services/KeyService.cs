using MemoryTrave.Maui.Infrastructure.Api;
using MemoryTrave.Maui.Infrastructure.Security;
using MemoryTrave.Maui.Models.Authorization;
using MemoryTrave.Maui.Services.Interfaces;

namespace MemoryTrave.Maui.Services;

public class KeyService(
    ApiRequestService apiService,
    IDialogService dialogService) : IKeyService
{
    private const string EncryptedPrivateKeyStorageName = "EncryptedPrivateKey";
    private const string PasswordStorageName = "JwtToken";
    
    public async Task GenerateKeys()
    {
        var keys = EccP256.GenerateKeyPair();
    
        var publicKeyBytes = EccP256.PublicKeyToBytes(keys.Public);
        var publicKey = Convert.ToBase64String(publicKeyBytes);
    
        var privateKeyBytes = EccP256.PrivateKeyToBytes(keys.Private);
        var  privateKey = Convert.ToBase64String(privateKeyBytes);
    
        var password = await SecureStorage.GetAsync(PasswordStorageName);

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
    
        var response = await apiService.PutRequest(URL.AddKeys, body);

        if (response.IsSuccess)
        {
            await SecureStorage.SetAsync(EncryptedPrivateKeyStorageName, encryptedPrivateKey);
        }
        else
            await dialogService.ShowMessage("Error", response.ErrorMessage);
    }
}