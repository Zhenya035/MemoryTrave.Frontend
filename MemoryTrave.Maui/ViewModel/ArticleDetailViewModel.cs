using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using MemoryTrave.Maui.Infrastructure.Api;
using MemoryTrave.Maui.Infrastructure.Security;
using MemoryTrave.Maui.Models.Articles;
using MemoryTrave.Maui.Models.Enums;
using MemoryTrave.Maui.Models.Photos;
using MemoryTrave.Maui.Resources.Localization;
using MemoryTrave.Maui.Services.Dialog;

namespace MemoryTrave.Maui.ViewModel;

[QueryProperty(nameof(ArticleId), "id")]
public partial class ArticleDetailViewModel(
    ApiRequestService apiService,
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
    private string _articleId = string.Empty;
    
    private Article _article = new();

    partial void OnArticleIdChanged(string value)
    {
        Task.Run(async () => await GetArticleAsync());
    }

    private async Task GetArticleAsync()
    {
        var article = await apiService.GetRequest<Article>(URL.GetArticleById(ArticleId));
        if (!article.IsSuccess && article.ErrorMessage != null)
        {
            await dialogService.ShowMessage(Localization.Error, article.ErrorMessage);
        }
        else if (article.IsSuccess && article.Data != null)
        {
            _article = article.Data;
            
            LastChange = _article.LastChange.ToString(CultureInfo.InvariantCulture);
            AuthorName = _article.AuthorName;
            LocationName = _article.LocationName;
            
            if (_article.Visibility == VisibilityEnum.Private && _article.EncryptedData != null &&
                _article.EncryptedKey != null)
            {
                Visibility = "Private";
                string decryptString;
                    
                if (_article.EncryptedData != null && _article.EncryptedKey != null)
                {
                    decryptString = AesGcm256.Decrypt(_article.EncryptedData, _article.EncryptedKey);
                }
                else
                    return;

                var decryptArticle = JsonSerializer.Deserialize<GetFullPrivateArticle>(decryptString);

                Description = decryptArticle.Description;
                
            }
            else if (_article.Visibility == VisibilityEnum.Public && _article.Description != null &&
                     _article.PhotosUrls != null)
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
                if (!photos.IsSuccess && photos.ErrorMessage != null)
                {
                    await dialogService.ShowMessage(Localization.Error, photos.ErrorMessage);
                    return;
                }
                
                await LoadPhotosAsync(photos.Data.Photos);
            }
        }
        else
            await dialogService.ShowMessage(Localization.Error, Localization.UnexpectedError);
    }
    
    private async Task LoadPhotosAsync(List<string> photoSting)
    {
        if (photoSting.Count == 0)
            return;

        try
        {
            foreach (var base64 in photoSting)
            {
                if (string.IsNullOrEmpty(base64))
                    continue;

                var photoBytes = Convert.FromBase64String(base64);
            
                var fileName = $"{Guid.NewGuid()}.jpg";
                var localPath = Path.Combine(FileSystem.CacheDirectory, fileName);
            
                await File.WriteAllBytesAsync(localPath, photoBytes);
            
                Photos.Add(localPath);
            }
        }
        catch (Exception ex)
        {
            await dialogService.ShowMessage(Localization.Error, $"Ошибка загрузки фото: {ex.Message}");
        }
    }
}