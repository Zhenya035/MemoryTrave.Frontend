using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MemoryTrave.Maui.Infrastructure.Api;
using MemoryTrave.Maui.Infrastructure.Security;
using MemoryTrave.Maui.Models.Authorization;
using MemoryTrave.Maui.Resources.Localization;
using MemoryTrave.Maui.Services.Auth;
using MemoryTrave.Maui.Services.Dialog;
using MemoryTrave.Maui.Services.Key;
using MemoryTrave.Maui.Services.Navigation;
using MemoryTrave.Maui.Services.PrivateKey;
using MemoryTrave.Maui.Services.Storage;

namespace MemoryTrave.Maui.ViewModel;

public partial class AuthViewModel(
    ApiRequestService apiService,
    IAuthService authService,
    INavigationService navigation,
    IDialogService dialogService,
    IKeyService keyService,
    IPrivateKeyService privateKeyService,
    IStorageService storageService) : ObservableObject
{
    [ObservableProperty] 
    private string? _username;
    
    [ObservableProperty]
    private string? _email;
    
    [ObservableProperty]
    private string? _password;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsRegistration))]
    private bool _isLogin = true;
    
    public bool IsRegistration => !IsLogin;

    [RelayCommand]
    private async Task Registration()
    {
        if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
        {
            await dialogService.ShowMessage(Localization.Error, Localization.FillError);
            return;
        }

        var body = new RegRequest
        {
            Username = Username,
            Email = Email,
            Password = Password
        };

        var authResponse = await apiService.PostRequest<RegRequest, AuthResponse>(URL.Registration(), body);

        if (authResponse.IsSuccess && authResponse.Data != null)
        {
            if (string.IsNullOrWhiteSpace(authResponse.Data.JwtToken))
            {
                await dialogService.ShowMessage(Localization.Error, Localization.JwtError);
            }

            var token =  authResponse.Data.JwtToken;

            storageService.LoadEmail(Email);
            await storageService.LoadTokenAsync(token);
            await authService.Login();
            apiService.SetJwtToken(token);
            
            var keys = await keyService.GenerateKeys(Password);

            var encryptedPrivateKey = GetEncryptedPrivateKey(keys.PrivateKey);
            
            var addKeyRequest = new AddKeyRequest
            {
                PublicKey = keys.PublicKey,
                EncryptedPrivateKey = encryptedPrivateKey
            };
    
            var response = await apiService.PutRequest(URL.AddKeys(), addKeyRequest);
            if (response.IsSuccess)
            {
                privateKeyService.SetKey(keys.PrivateKey);
            }
            else
            {
                await dialogService.ShowMessage(Localization.Error, Localization.UnexpectedError);
            }

            await navigation.GoBack();
        }
        else
            await dialogService.ShowMessage(Localization.Error, authResponse.ErrorMessage);

    }
    
    [RelayCommand]
    private async Task Login()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await dialogService.ShowMessage(Localization.Error, Localization.FillError);
            return;
        }

        var body = new AuthRequest
        {
            Email = Email,
            Password = Password
        };

        var authResponse = await apiService.PostRequest<AuthRequest, AuthResponse>(URL.Authorization(), body);

        if (authResponse.IsSuccess && authResponse.Data != null)
        {
            var token = authResponse.Data.JwtToken;
           
            storageService.LoadEmail(Email);
            await storageService.LoadTokenAsync(token);
            await authService.Login();
            apiService.SetJwtToken(token);
        }
        else
        {
            await dialogService.ShowMessage(Localization.Error, authResponse.ErrorMessage);
            return;
        }
        
        var privateKeyResponse = await apiService.GetRequest<GetKeyResponse>(URL.GetEncryptedPrivateKey());
        
        if (privateKeyResponse.IsSuccess && privateKeyResponse.Data != null)
        {
            var privateKey = DecryptPrivateKey(privateKeyResponse.Data.EncryptedPrivateKey);
            privateKeyService.SetKey(privateKey);
            
            await navigation.GoBack();
        }
        else
            await dialogService.ShowMessage(Localization.Error, privateKeyResponse.ErrorMessage);
    }
    
    [RelayCommand]
    private void OnRegisterTapped() =>
        IsLogin = false;
    
    [RelayCommand]
    private void OnLoginTapped() =>
        IsLogin = true;

    private string GetEncryptedPrivateKey(string privateKey)
    {
        var pbkdf2Data = Pbkdf2.DeriveKey(Password);
    
        var encryptedPrivateKey = AesGcm256.Encrypt(privateKey, pbkdf2Data.Key);
        var encryptedBytes = Convert.FromBase64String(encryptedPrivateKey);
        
        var saltLength = BitConverter.GetBytes(pbkdf2Data.Salt.Length);
        var saltSize = saltLength.Length;
        
        var combined = new byte[saltSize + pbkdf2Data.Salt.Length + encryptedBytes.Length];
        Buffer.BlockCopy(saltLength, 0, combined, 0, saltSize);
        Buffer.BlockCopy(pbkdf2Data.Salt, 0, combined, saltSize, 
            pbkdf2Data.Salt.Length);
        Buffer.BlockCopy(encryptedBytes, 0, combined, 
            saltSize + pbkdf2Data.Salt.Length,encryptedBytes.Length);
    
        encryptedPrivateKey = Convert.ToBase64String(combined);
        return encryptedPrivateKey;
    }

    private string DecryptPrivateKey(string privateKeyFromServer)
    {
        var combined = Convert.FromBase64String(privateKeyFromServer);
            
        var saltLength = BitConverter.ToInt32(combined, 0);
        var saltLengthSize = sizeof(int);
            
        var salt = new byte[saltLength];
        Buffer.BlockCopy(combined, saltLengthSize, salt, 0, saltLength);

        var encryptedData = new byte[combined.Length - saltLengthSize - saltLength];
        Buffer.BlockCopy(combined, saltLengthSize + saltLength, encryptedData, 0,
            encryptedData.Length);

        var encryptedStringFromServer = Convert.ToBase64String(encryptedData);

        var dekBytes = Pbkdf2.DeriveKey(Password, salt).Key;
        var dek = Convert.ToBase64String(dekBytes);
            
        var privateKey = AesGcm256.Decrypt(encryptedStringFromServer, dek);
        
        return privateKey;
    }
}