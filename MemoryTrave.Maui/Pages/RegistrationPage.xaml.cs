using MemoryTrave.Maui.Api;
using MemoryTrave.Maui.Response;
using MemoryTrave.Maui.Security;

namespace MemoryTrave.Maui.Pages;

public partial class RegistrationPage : ContentPage
{
    private ApiRequestService _apiRequest;
    
    public RegistrationPage(ApiRequestService apiRequest)
    {
        InitializeComponent();
        _apiRequest = apiRequest;
    }

    private async void Registration(object? sender, EventArgs e)
    {
        var username = UsernameEntry.Text;
        var email = EmailEntry.Text;
        var password = PasswordEntry.Text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Error", "Пожалуйста заполните все поля", "OK");
            return;
        }

        var body = new RegRequest
        {
            Username = username,
            Email = email,
            Password = password
        };

        var authResponse = await _apiRequest.PostRequest<RegRequest, AuthResponse>(URL.Registration, body);

        if (authResponse.IsSuccess)
        {
            await SecureStorage.Default.SetAsync("JwtToken", authResponse.Data.JwtToken);
            await SecureStorage.Default.SetAsync("Password", password);
            _apiRequest.SetJwtToken(authResponse.Data.JwtToken);
            
            CreateEccKeys();
            
            Application.Current.MainPage = new AppShell();
        }
        else
            await DisplayAlert("Error", authResponse.ErrorMessage.ToUserFriendlyMessage(), "OK");
    }

    private async void CreateEccKeys()
    {
        var keys = EccP256.GenerateKeyPair();
        
        var publicKeyBytes = EccP256.PublicKeyToBytes(keys.Public);
        var publicKey = Convert.ToBase64String(publicKeyBytes);
        
        var privateKeyBytes = EccP256.PrivateKeyToBytes(keys.Private);
        var  privateKey = Convert.ToBase64String(privateKeyBytes);
        
        var password = await SecureStorage.GetAsync("Password");

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
        
        var response = await _apiRequest.PutRequest(URL.Registration, body);

        if (response.IsSuccess)
        {
            await SecureStorage.SetAsync("EncryptedPrivateKey", encryptedPrivateKey);
        }
        else
            await DisplayAlert("Error", response.ErrorMessage.ToUserFriendlyMessage(), "OK");

    }
}