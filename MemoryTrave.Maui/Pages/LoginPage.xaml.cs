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

        var response = await _apiRequest.PostRequest<AuthRequest, AuthResponse>(URL.Login, body);

        if (response.IsSuccess)
        {
            await SecureStorage.SetAsync("JwtToken", response.Data.JwtToken);
            Application.Current.MainPage = new AppShell();
        }
        else
        {
            await DisplayAlert("Error", response.ErrorMessage.ToUserFriendlyMessage(), "OK");
        }
    }
}