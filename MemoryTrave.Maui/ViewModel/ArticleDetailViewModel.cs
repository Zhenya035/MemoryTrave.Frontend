using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MemoryTrave.Maui.Infrastructure.Api;
using MemoryTrave.Maui.Infrastructure.Security;
using MemoryTrave.Maui.Models.Articles;
using MemoryTrave.Maui.Models.Enums;
using MemoryTrave.Maui.Models.Photos;
using MemoryTrave.Maui.Resources.Localization;
using MemoryTrave.Maui.Services.Dialog;
using MemoryTrave.Maui.Services.Error;
using MemoryTrave.Maui.Services.Photo;
using MemoryTrave.Maui.Services.PrivateKey;
using MemoryTrave.Maui.Services.SavePhoto;
using MemoryTrave.Maui.Services.Storage;

namespace MemoryTrave.Maui.ViewModel;

[QueryProperty(nameof(ArticleId), "id")]
public partial class ArticleDetailViewModel(
    ApiRequestService apiService,
    IPhotoService photoService,
    IPrivateKeyService privateKeyService,
    IConvertErrorService errorService,
    IPhotoSaveService  photoSaveService,
    IStorageService storageService,
    IDialogService dialogService) : ObservableObject
{
    [ObservableProperty]
    private string _visibility = string.Empty;
    
    [ObservableProperty]
    private string _lastChange = string.Empty;
    
    [ObservableProperty]
    private string _authorName = string.Empty;
    
    [ObservableProperty]
    private string _locationName = string.Empty;
    
    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty] 
    private ObservableCollection<string> _photos = [];

    [ObservableProperty] 
    private ObservableCollection<object> _selectedPhotos = [];
   
    [ObservableProperty] 
    private string _articleId = string.Empty;
    
    private Article _article = new();

    [ObservableProperty] 
    private bool _isAuthor;

    partial void OnArticleIdChanged(string value)
    {
        Task.Run(async () => await GetArticleAsync());
    }

    private async Task GetArticleAsync()
    {
        var article = await apiService.GetRequest<Article>(URL.GetArticleById(ArticleId));
        if (!article.IsSuccess && article.ErrorMessage != null && article.StatusCode != null)
        {
            await dialogService.ShowMessage(Localization.Error, errorService.ConvertError(article.StatusCode));
        }
        else if (article.IsSuccess && article.Data != null)
        {
            _article = article.Data;
            
            LastChange = _article.LastChange.ToString(CultureInfo.InvariantCulture);
            AuthorName = _article.AuthorName;
            LocationName = _article.LocationName;
            
            var userId = await storageService.GetUserIdAsync();
            if (_article.AuthorId.ToString() == userId)
                IsAuthor = true;
            
            if (_article.Visibility == VisibilityEnum.Private && _article.EncryptedDescription != null &&
                _article.EncryptedKey != null)
            {
                Visibility = "Private";

                var privateKeyString = privateKeyService.GetKey();
                if (privateKeyString == null)
                {
                    await dialogService.ShowMessage(Localization.Error, Localization.UnexpectedError);
                    return;
                }
                
                var privateKey = EccP256.StringToPrivateKey(privateKeyString);
                var encryptedDek = Convert.FromBase64String(_article.EncryptedKey);

                var dekBytes = EccP256.Decrypt(privateKey, encryptedDek);
                var dek = Convert.ToBase64String(dekBytes);
                
                var decryptString = AesGcm256.Decrypt(_article.EncryptedDescription, dek);

                var decryptArticle = JsonSerializer.Deserialize<PrivateArticle>(decryptString);

                Description = decryptArticle.Description;
                
                var getPhotoRequest = new GetPhotosByArticle
                {
                    ArticleId = _article.Id,
                    Author = AuthorName
                };

                var photos = await apiService.PostRequest<GetPhotosByArticle, PhotoList>
                    (URL.GetPhotosFromArticle(), getPhotoRequest);

                if (!photos.IsSuccess && photos.ErrorMessage != null && photos.StatusCode != null)
                {
                    await dialogService.ShowMessage(Localization.Error, errorService.ConvertError(photos.StatusCode));
                    return;
                }
                
                var decryptedPhotos = photos.Data.Photos.Select(photo => 
                    AesGcm256.Decrypt(photo, dek)).ToList();

                await GetPhotosAsync(decryptedPhotos);
            }
            else if (_article.Visibility == VisibilityEnum.Public && _article.Description != null)
            {
                Visibility = "Public";
                Description = _article.Description;

                var getPhotoRequest = new GetPhotosByArticle
                {
                    ArticleId = _article.Id,
                    Author = AuthorName
                };
                var photos = await apiService.PostRequest<GetPhotosByArticle, PhotoList>
                    (URL.GetPhotosFromArticle(), getPhotoRequest);
                if (!photos.IsSuccess && photos.ErrorMessage != null && photos.StatusCode != null)
                {
                    await dialogService.ShowMessage(Localization.Error, errorService.ConvertError(photos.StatusCode));
                    return;
                }
                
                await GetPhotosAsync(photos.Data.Photos);
            }
        }
        else
            await dialogService.ShowMessage(Localization.Error, Localization.UnexpectedError);
    }

    private async Task GetPhotosAsync(List<string> photos)
    {
        if (photos.Count == 0)
            return;

        try
        {
            var photosList = await photoService.AddPhotosToLocalAsync(photos, _article.Id.ToString());
            Photos = new ObservableCollection<string>(photosList);
        }
        catch (Exception ex)
        {
            await dialogService.ShowMessage(Localization.Error, Localization.PhotoUploadError);
        }
    }

    [RelayCommand]
    private async Task OpenPhotoAsync(string photoPath)
    {
        if (string.IsNullOrEmpty(photoPath))
            return;

        try
        {
            var contentPage = new ContentPage
            {
                BackgroundColor = Colors.Black
            };

            var image = new Image
            {
                Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };

            var bytes = await File.ReadAllBytesAsync(photoPath);
            image.Source = ImageSource.FromStream(() => new MemoryStream(bytes));

            var closeButton = new Button
            {
                Text = "✕",
                FontSize = 24,
                TextColor = Colors.White,
                BackgroundColor = Colors.Transparent,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Start,
                Margin = new Thickness(20, 40, 20, 0),
                WidthRequest = 50,
                HeightRequest = 50,
                ZIndex = 1
            };

            var grid = new Grid();
            grid.Children.Add(image);
            grid.Children.Add(closeButton);

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += async (s, e) => await contentPage.Navigation.PopModalAsync();
            image.GestureRecognizers.Add(tapGesture);

            closeButton.Clicked += async (s, e) => await contentPage.Navigation.PopModalAsync();

            contentPage.Content = grid;

            await Shell.Current.Navigation.PushModalAsync(contentPage);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error opening photo: {ex.Message}");
            await dialogService.ShowMessage(Localization.Error, "Не удалось открыть изображение");
        }
    }

    [RelayCommand]
    private async Task DownloadSelectedPhotosAsync()
    {
        if (SelectedPhotos.Count == 0)
        {
            await dialogService.ShowMessage(Localization.Error, "Выберите фото");
            return;
        }

        try
        {
            var paths = SelectedPhotos.Cast<string>().ToList();
            await photoSaveService.DownloadPhotoAsync(paths);
            
            await dialogService.ShowMessage("Успех", "Все фотографии успешно сохранены");
        }
        catch (Exception e)
        {
            await dialogService.ShowMessage(Localization.Error, Localization.UnexpectedError);
        }
    }
}