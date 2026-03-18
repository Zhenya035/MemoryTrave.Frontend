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
using MemoryTrave.Maui.Services.Dialog;
using MemoryTrave.Maui.Services.Navigation;
using MemoryTrave.Maui.Services.Photo;
using MemoryTrave.Maui.Services.PrivateKey;
using MemoryTrave.Maui.View;

namespace MemoryTrave.Maui.ViewModel;

[QueryProperty(nameof(LocationId), "id")]
public partial class LocationDetailViewModel(
    INavigationService navigation,
    IDialogService dialogService,
    IPrivateKeyService  privateKeyService,
    ApiRequestService apiService) : ObservableObject
{
    [ObservableProperty]
    private string _locationId;
    
    [ObservableProperty]
    private string _locationName;
    
    [ObservableProperty]
    private ObservableCollection<ArticleForLocation> _articles;
    
    [RelayCommand]
    private async Task ToArticleAsync(Guid articleId)
    {
        await navigation.GoTo($"{nameof(ArticleDetailPage)}?id={articleId.ToString()}");
    }

    [RelayCommand]
    private async Task ToAddArticleAsync()
    {
        await navigation.GoTo($"{nameof(AddArticlePage)}?id={LocationId}");
    }

    partial void OnLocationIdChanged(string value)
    {
        Task.Run(async () => await GetLocationAsync());
    }

    private async Task GetLocationAsync()
    {
        var result = await apiService.GetRequest<LocationForDetail>(URL.GetLocationById(LocationId));
        if(!result.IsSuccess && result.ErrorMessage != null)
            await dialogService.ShowMessage(Localization.Error, result.ErrorMessage);
        else if (result.IsSuccess && result.Data != null)
        {
            var location = result.Data;
            LocationName =  location.Name;
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
                else
                {
                    string decryptString;
                    
                    if (article.EncryptedDescription != null && article.EncryptedKey != null)
                    {
                        var privateKeyString = privateKeyService.GetKey();
                        var privateKey = EccP256.StringToPrivateKey(privateKeyString);
                        
                        var encryptedDekBytes = Convert.FromBase64String(article.EncryptedKey);
                        var dekBytes = EccP256.Decrypt(privateKey, encryptedDekBytes);
                        var dek = Convert.ToBase64String(dekBytes);
                        
                        decryptString = AesGcm256.Decrypt(article.EncryptedDescription, dek);
                    }
                    else
                        return;

                    var decryptArticle = JsonSerializer.Deserialize<PrivateArticle>(decryptString);

                    newArticle.Description = decryptArticle.Description;
                }
                articles.Add(newArticle);
            }
            
            Articles = new ObservableCollection<ArticleForLocation>(articles);
        }
        else
            await dialogService.ShowMessage(Localization.Error, Localization.UnexpectedError);
    }
}