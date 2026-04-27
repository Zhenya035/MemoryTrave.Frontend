using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MemoryTrave.Maui.Infrastructure.Api;
using MemoryTrave.Maui.Infrastructure.Security;
using MemoryTrave.Maui.Models;
using MemoryTrave.Maui.Models.Articles;
using MemoryTrave.Maui.Models.Articles.Access;
using MemoryTrave.Maui.Models.Enums;
using MemoryTrave.Maui.Models.Friends;
using MemoryTrave.Maui.Resources.Localization;
using MemoryTrave.Maui.Services.Dialog;
using MemoryTrave.Maui.Services.Navigation;
using MemoryTrave.Maui.Services.PrivateKey;
using MemoryTrave.Maui.View;

namespace MemoryTrave.Maui.ViewModel;

public partial class FriendsViewModel(
    ApiRequestService apiService,
    IPrivateKeyService privateKeyService, 
    INavigationService navigation,
    IDialogService dialogService) : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<User> _friends = [];

    [ObservableProperty]
    private ObservableCollection<User> _requests = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(InRequests))]
    private bool _inFriendship = true;

    public bool InRequests => !InFriendship;
    
    [RelayCommand]
    private void RequestsTapped() =>
        InFriendship = false;
    
    [RelayCommand]
    private void FriendshipTapped() =>
        InFriendship = true;

    [RelayCommand]
    private async Task ConfirmRequestAsync(Guid id)
    {
        var result = await apiService.PostRequest<GetId>(URL.ConfirmRequest(id.ToString()));
        if (!result.IsSuccess && result.ErrorMessage != null)
            await dialogService.ShowMessage(Localization.Error, result.ErrorMessage);
        else if (result.IsSuccess && result.Data != null)
        {
            var deleteObject = Requests.First(f => f.Id == id);
            Requests.Remove(deleteObject);
            Friends.Add(deleteObject);

            var privateArticlesResult = await apiService.GetRequest<List<GetPrivateForFriend>>(URL.GetMyPrivate());
            if(!privateArticlesResult.IsSuccess && privateArticlesResult.ErrorMessage != null)
            {
                await dialogService.ShowMessage(Localization.Error, privateArticlesResult.ErrorMessage);
                return;
            }
            else if (!privateArticlesResult.IsSuccess)
            {
                await dialogService.ShowMessage(Localization.Error, Localization.UnexpectedError);
                return;
            }
            
            var friendId = result.Data.Id;
            var friendPublicKeyResult = await apiService.GetRequest<GetPublicKey>(URL.GetPublicKeyById(friendId.ToString()));
            if (!friendPublicKeyResult.IsSuccess && friendPublicKeyResult.ErrorMessage != null)
            {
                await dialogService.ShowMessage(Localization.Error, friendPublicKeyResult.ErrorMessage);
                return;
            }
            else if (!friendPublicKeyResult.IsSuccess)
            {   
                await dialogService.ShowMessage(Localization.Error, Localization.UnexpectedError);
                return; 
            }
            
            var privateArticles = privateArticlesResult.Data;
            
            var friendPublicKeyString = friendPublicKeyResult.Data.PublicKey;
            var friendPublicKey = EccP256.StringToPublicKey(friendPublicKeyString);
            
            var privateKeyString = privateKeyService.GetKey();
            var privateKey = EccP256.StringToPrivateKey(privateKeyString);

            var access = new List<AddEncryptedKeysForAddFriend>();
            foreach (var article in privateArticles)
            {
                var encryptedDek = Convert.FromBase64String(article.EncryptedKey);;
                var dek = EccP256.Decrypt(privateKey, encryptedDek);
                
                var encryptedDekForFriend = EccP256.Encrypt(friendPublicKey, dek);

                var newAccess = new AddEncryptedKeysForAddFriend()
                {
                    EncryptedKey = Convert.ToBase64String(encryptedDekForFriend),
                    ArticleId = article.ArticleId
                };
                
                access.Add(newAccess);
            }
            
            var addResponse = await apiService.PostRequest<List<AddEncryptedKeysForAddFriend>, bool>
                (URL.AddAccessForUser(friendId.ToString()), access);
            
            if (!addResponse.IsSuccess &&  addResponse.ErrorMessage != null)
                await dialogService.ShowMessage(Localization.Error, addResponse.ErrorMessage);
            else if (!addResponse.IsSuccess)
                await dialogService.ShowMessage(Localization.Error, Localization.UnexpectedError);
        }
        else
            await dialogService.ShowMessage(Localization.Error, Localization.UnexpectedError);
    }
    
    [RelayCommand]
    private async Task CancelRequest(Guid id)
    {
        var result = await apiService.DeleteRequest(URL.CancelRequest(id.ToString()));
        if (!result.IsSuccess && result.ErrorMessage != null)
            await dialogService.ShowMessage(Localization.Error, result.ErrorMessage);
        else if (result.IsSuccess)
        {
            var deleteObject = Requests.First(f => f.Id == id);
            Requests.Remove(deleteObject);
        }
        else
            await dialogService.ShowMessage(Localization.Error, Localization.UnexpectedError);
    }
    
    [RelayCommand]
    private async Task DeleteFriendship(Guid id)
    {
        var result = await apiService.DeleteRequest(URL.DeleteFriendship(id.ToString()));
        if (!result.IsSuccess && result.ErrorMessage != null)
            await dialogService.ShowMessage(Localization.Error, result.ErrorMessage);
        else if (result.IsSuccess)
        {
            var deleteObject = Friends.First(f => f.Id == id);
            Friends.Remove(deleteObject);
        }
        else
            await dialogService.ShowMessage(Localization.Error, Localization.UnexpectedError);
    }

    [RelayCommand]
    private async Task GoToFindFriends()
    {
        await navigation.GoTo(nameof(FindFriendsPage));
    }
    
    public async Task GetFriendsAsync()
    {
        var friendsResult = await apiService.GetRequest<List<User>>(URL.GetFriends());
        var toMeRequestResult =
            await apiService.GetRequest<List<User>>(URL.GetRequests((int)DirectionRequestEnum.Incoming));
        
        if(!friendsResult.IsSuccess && friendsResult.ErrorMessage != null)
            await dialogService.ShowMessage(Localization.Error, friendsResult.ErrorMessage);
        else if(!toMeRequestResult.IsSuccess && toMeRequestResult.ErrorMessage != null)
            await dialogService.ShowMessage(Localization.Error, toMeRequestResult.ErrorMessage);
        else if (friendsResult.IsSuccess && friendsResult.Data != null && 
                 toMeRequestResult.IsSuccess && toMeRequestResult.Data != null)
        {
            Friends = new ObservableCollection<User>(friendsResult.Data);
            Requests = new ObservableCollection<User>(toMeRequestResult.Data);
        }
        else
            await dialogService.ShowMessage(Localization.Error, Localization.UnexpectedError);
    }
}