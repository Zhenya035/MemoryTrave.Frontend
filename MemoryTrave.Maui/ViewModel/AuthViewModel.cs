using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MemoryTrave.Maui.Infrastructure.Api;
using MemoryTrave.Maui.Models.Authorization;
using MemoryTrave.Maui.Services.Interfaces;

namespace MemoryTrave.Maui.ViewModel;

public partial class AuthViewModel(
    ApiRequestService apiService,
    IAuthService authService,
    INavigationService navigation,
    IDialogService dialogService,
    IKeyService keyService) : ObservableObject
{
    private const string PasswordStorageName = "Password";
    
    [ObservableProperty] 
    private string username;
    
    [ObservableProperty]
    private string email;
    
    [ObservableProperty]
    private string password;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsRegistration))]
    private bool isLogin = true;
    
    public bool IsRegistration => !IsLogin;

    [RelayCommand]
    private async Task Registration()
    {
        if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
        {
            await dialogService.ShowMessage("Error", "Пожалуйста заполните все поля");
            return;
        }

        var body = new RegRequest
        {
            Username = Username,
            Email = Email,
            Password = Password
        };

        var authResponse = await apiService.PostRequest<RegRequest, AuthResponse>(URL.Registration(), body);

        if (authResponse.IsSuccess)
        {
            await SecureStorage.Default.SetAsync(PasswordStorageName, Password);
            
            await authService.Login(authResponse.Data.JwtToken);
            apiService.SetJwtToken(authResponse.Data.JwtToken);
            
            await keyService.GenerateKeys();
            
            await navigation.GoBack();
        }
        else
            await dialogService.ShowMessage("Error", authResponse.ErrorMessage);

    }
    
    [RelayCommand]
    private async Task Login()
    {
        if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
        {
            await dialogService.ShowMessage("Error", "Пожалуйста заполните все поля");
            return;
        }

        var body = new AuthRequest
        {
            Email = Email,
            Password = Password
        };

        var authResponse = await apiService.PostRequest<AuthRequest, AuthResponse>(URL.Authorization(), body);

        if (authResponse.IsSuccess)
        {
            await authService.Login(authResponse.Data.JwtToken);
            apiService.SetJwtToken(authResponse.Data.JwtToken);
        }
        else
        {
            await dialogService.ShowMessage("Error", authResponse.ErrorMessage);
            return;
        }
        
        var privateKeyResponse = await apiService.GetRequest<GetKeyResponse>(URL.GetEncryptedPrivateKey());
        
        if (privateKeyResponse.IsSuccess)
        {
            await SecureStorage.Default.SetAsync("EncryptedPrivateKey", privateKeyResponse.Data.EncryptedPrivateKey);

            await navigation.GoBack();
        }
        else
            await dialogService.ShowMessage("Error", privateKeyResponse.ErrorMessage);
    }
    
    [RelayCommand]
    private void OnRegisterTapped() =>
        IsLogin = false;
    
    [RelayCommand]
    private void OnLoginTapped() =>
        IsLogin = true;
}