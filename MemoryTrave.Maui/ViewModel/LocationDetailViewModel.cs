using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MemoryTrave.Maui.Infrastructure.Api;
using MemoryTrave.Maui.Infrastructure.Security;
using MemoryTrave.Maui.Models.Articles;
using MemoryTrave.Maui.Models.Enums;
using MemoryTrave.Maui.Models.Location;
using MemoryTrave.Maui.Resources.Localization;
using MemoryTrave.Maui.Services.Auth;
using MemoryTrave.Maui.Services.Dialog;
using MemoryTrave.Maui.Services.Error;
using MemoryTrave.Maui.Services.Navigation;
using MemoryTrave.Maui.Services.PrivateKey;
using MemoryTrave.Maui.View;

namespace MemoryTrave.Maui.ViewModel;

public partial class LocationDetailViewModel : ObservableObject
{
    [ObservableProperty]
    private string _locationName;
    
    [ObservableProperty]
    private ObservableCollection<ArticleForLocation> _articles;
    
    [ObservableProperty]
    private bool _isAuthorized;

    private readonly INavigationService _navigation;
    private readonly IDialogService _dialogService;
    private readonly IPrivateKeyService _privateKeyService;
    private readonly IAuthService _authService;
    private readonly IConvertErrorService _errorService;
    private readonly ApiRequestService _apiService;

    private string _currentLocationId;

    public LocationDetailViewModel(
        INavigationService navigation,
        IDialogService dialogService,
        IPrivateKeyService privateKeyService,
        IAuthService authService,
        IConvertErrorService errorService,
        ApiRequestService apiService)
    {
        _navigation = navigation;
        _dialogService = dialogService;
        _privateKeyService = privateKeyService;
        _authService = authService;
        _errorService = errorService;
        _apiService = apiService;
        _isAuthorized = authService.IsAuthorized;
    }

    [RelayCommand]
    private async Task ToArticleAsync(Guid articleId)
    {
        await _navigation.GoTo($"{nameof(ArticleDetailPage)}?id={articleId.ToString()}");
    }

    [RelayCommand]
    private async Task ToAddArticleAsync()
    {
        await _navigation.GoTo($"{nameof(AddArticlePage)}?id={_currentLocationId}");
    }

    public async Task LoadLocationAsync(string locationId)
    {
        if (string.IsNullOrEmpty(locationId)) return;
        
        _currentLocationId = locationId;

        var result = await _apiService.GetRequest<LocationForDetail>(URL.GetLocationById(locationId));

        if (result.IsSuccess && result.Data != null)
        {
            var location = result.Data;
            var articles = new List<ArticleForLocation>();

            foreach (var article in location.Articles)
            {
                var newArticle = new ArticleForLocation
                {
                    Id = article.Id,
                    Visibility = article.Visibility,
                    LastChange = article.LastChange,
                    CreatedAt = article.CreatedAt,
                    AuthorName = article.AuthorName,
                };

                if (article.Visibility == VisibilityEnum.Public)
                {
                    newArticle.Description = article.Description;
                }
                else if (article.EncryptedDescription != null && article.EncryptedKey != null)
                {
                    try
                    {
                        var privateKeyString = _privateKeyService.GetKey();
                        var privateKey = EccP256.StringToPrivateKey(privateKeyString);
                        var encryptedDekBytes = Convert.FromBase64String(article.EncryptedKey);
                        var dekBytes = EccP256.Decrypt(privateKey, encryptedDekBytes);
                        var dek = Convert.ToBase64String(dekBytes);
                        var decryptString = AesGcm256.Decrypt(article.EncryptedDescription, dek);
                        var decryptArticle = JsonSerializer.Deserialize<PrivateArticle>(decryptString);
                        if (decryptArticle != null)
                            newArticle.Description = decryptArticle.Description;
                    }
                    catch { }
                }
                articles.Add(newArticle);
            }

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                LocationName = location.Name;
                Articles = new ObservableCollection<ArticleForLocation>(articles);
            });
        }
        else if (!result.IsSuccess && result.ErrorMessage != null && result.StatusCode != null)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
                await _dialogService.ShowMessage(Localization.Error, _errorService.ConvertError(result.StatusCode)));
        }
    }
}