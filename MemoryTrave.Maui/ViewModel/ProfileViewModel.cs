using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MemoryTrave.Maui.Infrastructure.Api;
using MemoryTrave.Maui.Infrastructure.Security;
using MemoryTrave.Maui.Models.Articles;
using MemoryTrave.Maui.Models.Profile;
using MemoryTrave.Maui.Resources.Localization;
using MemoryTrave.Maui.Services.Dialog;
using MemoryTrave.Maui.Services.Navigation;
using MemoryTrave.Maui.Services.PrivateKey;
using MemoryTrave.Maui.View;

namespace MemoryTrave.Maui.ViewModel;

public partial class ProfileViewModel(
    ApiRequestService apiService,
    IDialogService dialogService,
    IPrivateKeyService privateKeyService,
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
    private async Task ToArticle(Guid articleId)
    {
        await navigation.GoTo($"{nameof(ArticleDetailPage)}?id={articleId.ToString()}");
    }

    public async Task GetProfileAsync()
    {
        var result = await apiService.GetRequest<MyProfile>(URL.GetProfile());

        switch (result.IsSuccess)
        {
            case false when result.ErrorMessage != null:
                await dialogService.ShowMessage(Localization.Error, result.ErrorMessage);
                return;
            case true when result.Data == null:
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
            var articleForAdd = new ProfileArticles
            {
                Id = article.Id,
                LocationName = article.LocationName,
                CreatedAt = article.CreatedAt,
                LastChange = article.LastChange,
            };
            
            if (article.IsPrivate)
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

                articleForAdd.Visibility = "Private";
                articleForAdd.Description = decryptArticle.Description;
            }
            else
            {
                articleForAdd.Visibility = "Public";
                articleForAdd.Description = article.Description;
            }
            articles.Add(articleForAdd);
        }

        Articles = new ObservableCollection<ProfileArticles>(articles);
    }
}