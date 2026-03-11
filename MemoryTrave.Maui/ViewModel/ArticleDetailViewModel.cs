using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using MemoryTrave.Maui.Infrastructure.Api;
using MemoryTrave.Maui.Models.Articles;
using MemoryTrave.Maui.Models.Enums;
using MemoryTrave.Maui.Resources.Localization;
using MemoryTrave.Maui.Services.Interfaces;

namespace MemoryTrave.Maui.ViewModel;

[QueryProperty(nameof(_articleId), "id")]
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
    private List<string> _photosUrls = []; 
   
    [ObservableProperty] 
    private string _articleId = string.Empty;
    
    private Article _article = new();

    partial void OnArticleIdChanged(string value)
    {
        Task.Run(async () => await GetArticleAsync());
    }

    private async Task GetArticleAsync()
    {
        var article = await apiService.GetRequest<Article>(_articleId);
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
            
            if (_article.Visibility == VisibilityEnum.Private)
            {
                Visibility = "Private";
            }
            else if (_article.Visibility == VisibilityEnum.Public && _article.Description != null &&
                     _article.PhotosUrls != null)
            {
                Visibility = "Public";
                Description = _article.Description;
                PhotosUrls = _article.PhotosUrls;
            }
        }
        else
            await dialogService.ShowMessage(Localization.Error, Localization.UnexpectedError);
    }
}