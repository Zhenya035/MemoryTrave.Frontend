using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MemoryTrave.Maui.Infrastructure.Api;
using MemoryTrave.Maui.Models.Authorization;
using MemoryTrave.Maui.Resources.Localization;
using MemoryTrave.Maui.Services.Interfaces;

namespace MemoryTrave.Maui.ViewModel;

public partial class AuthViewModel(
    ApiRequestService apiService,
    IAuthService authService,
    INavigationService navigation,
    IDialogService dialogService,
    IKeyService keyService,
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
            await storageService.LoadTokenAsync(token);
            await authService.Login();
            apiService.SetJwtToken(token);
            
            var privateKey = await keyService.GenerateKeys(Password);
            if(!privateKey.IsSuccess && privateKey.Error != null)
            {
                await dialogService.ShowMessage(Localization.Error, privateKey.Error);
                return;
            }

            if (privateKey.IsSuccess && privateKey.EncryptedPrivateKey != null &&
                privateKey.EncryptedPasswordKey != null)
            {
                await storageService.LoadPrivateKeyAsync(privateKey.EncryptedPrivateKey);
                await storageService.LoadPasswordDekAsync(privateKey.EncryptedPasswordKey);
            }
            else
            {
                dialogService.ShowMessage(Localization.Error, Localization.UnexpectedError);
            }

            await navigation.GoBack();
        }
        else
            await dialogService.ShowMessage(Localization.Error, authResponse.ErrorMessage);//todo создать сервис ответов на коды

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
            await storageService.LoadPrivateKeyAsync(privateKeyResponse.Data.EncryptedPrivateKey);
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
}