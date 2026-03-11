using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MemoryTrave.Maui.Infrastructure.Api;
using MemoryTrave.Maui.Models.Profile;
using MemoryTrave.Maui.Resources.Localization;
using MemoryTrave.Maui.Services.Interfaces;
using MemoryTrave.Maui.View;

namespace MemoryTrave.Maui.ViewModel;

public partial class ProfileViewModel(
    ApiRequestService apiService,
    IDialogService dialogService,
    INavigationService navigation) : ObservableObject
{
    [ObservableProperty] 
    private string? _username;
    
    [ObservableProperty] 
    private string? _email;
    
    [ObservableProperty]
    private int _friendsCount;

    [ObservableProperty]
    private int _articleCount;

    [ObservableProperty] 
    private ObservableCollection<ProfileArticles>? _articles = [];

    [RelayCommand]
    private async Task ToFriends()
    {
        await navigation.GoTo(nameof(FriendsPage));
    }

    [RelayCommand]
    private async Task ToArticle(int articleId)
    {
        await navigation.GoTo(nameof(ArticleDetailPage) + "?id={articleId}");
    }

    public async Task GetProfileAsync()
    {
        var result = await apiService.GetRequest<MyProfile>(URL.GetProfile());

        if (!result.IsSuccess && result.ErrorMessage != null)
        {
            await dialogService.ShowMessage(Localization.Error, result.ErrorMessage);
            return;
        }
        else if (result.IsSuccess && result.Data == null)
        {
            await dialogService.ShowMessage(Localization.Error, Localization.UnexpectedError);
            return;
        }
        
        var profile = result.Data;

        Username = profile.Username;
        Email = profile.Email;
        FriendsCount = profile.FriendsCount;
        ArticleCount = profile.ArticlesCount;

        var articles = new List<ProfileArticles>();
        foreach (var article in profile.Articles)
        {
            if (article.IsPrivate)
            {
                var description = article.EncryptedPreviewData; //todo добавить расшифровку
                
                articles.Add(new ProfileArticles
                {
                    Id = article.Id,
                    LocationName = article.LocationName,
                    CreatedAt =  article.CreatedAt,
                    LastChange =  article.LastChange,
                    IsPrivate = "Private",
                    Description = description
                });
            }
            else
            {
                articles.Add(new ProfileArticles
                {
                    Id = article.Id,
                    LocationName = article.LocationName,
                    CreatedAt =  article.CreatedAt,
                    LastChange =  article.LastChange,
                    IsPrivate = "Public",
                    Description = article.Description
                });
            }
        }

        Articles = new ObservableCollection<ProfileArticles>(articles);
    }
}