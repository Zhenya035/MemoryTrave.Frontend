using MemoryTrave.Maui.Api;
using MemoryTrave.Maui.Response;

namespace MemoryTrave.Maui.Pages;

public partial class LoginPage : ContentPage
{
    private ApiRequestService _apiRequest;
    
    public LoginPage(ApiRequestService apiRequestService)
    {
        InitializeComponent();
        _apiRequest = apiRequestService;
    }

    private async void Login(object? sender, EventArgs e)
    {
        var email = EmailEntry.Text;
        var password = PasswordEntry.Text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Error", "Пожалуйста заполните все поля", "OK");
            return;
        }

        var body = new AuthRequest
        {
            Email = email,
            Password = password
        };

        var authResponse = await _apiRequest.PostRequest<AuthRequest, AuthResponse>(URL.Login, body);

        if (authResponse.IsSuccess)
        {
            await SecureStorage.Default.SetAsync("JwtToken", authResponse.Data.JwtToken);
            _apiRequest.SetJwtToken(authResponse.Data.JwtToken);
        }
        else
        {
            await DisplayAlert("Error", authResponse.ErrorMessage.ToUserFriendlyMessage(), "OK");
            return;
        }
        
        var privateKeyResponse = await _apiRequest.GetRequest<GetKeyResponse>(URL.GetPrivateKey);
        
        if (privateKeyResponse.IsSuccess)
        {
            await SecureStorage.Default.SetAsync("EncryptedPrivateKey", privateKeyResponse.Data.EncryptedPrivateKey);
            Application.Current.MainPage = new AppShell();
        }
        else
            await DisplayAlert("Error", privateKeyResponse.ErrorMessage.ToUserFriendlyMessage(), "OK");
    }

    private void OnRegisterTapped(object? sender, TappedEventArgs e)
    {
        Application.Current.MainPage = new RegistrationPage(_apiRequest);
    }
}