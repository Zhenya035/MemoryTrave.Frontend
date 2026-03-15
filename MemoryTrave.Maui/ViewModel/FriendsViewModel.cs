using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MemoryTrave.Maui.Infrastructure.Api;
using MemoryTrave.Maui.Models.Enums;
using MemoryTrave.Maui.Models.Friends;
using MemoryTrave.Maui.Resources.Localization;
using MemoryTrave.Maui.Services.Interfaces;

namespace MemoryTrave.Maui.ViewModel;

public partial class FriendsViewModel(
    ApiRequestService apiService,
    IDialogService dialogService) : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Friend> _friends = [];

    [ObservableProperty]
    private ObservableCollection<Friend> _requests = [];

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
        var result = await apiService.PostRequest(URL.ConfirmRequest(id.ToString()));
        if (!result.IsSuccess && result.ErrorMessage != null)
            await dialogService.ShowMessage(Localization.Error, result.ErrorMessage);
        else if (result.IsSuccess)
        {
            var deleteObject = Requests.First(f => f.Id == id);
            Requests.Remove(deleteObject);
            Friends.Add(deleteObject);
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

    public async Task GetFriendsAsync()
    {
        var friendsResult = await apiService.GetRequest<List<Friend>>(URL.GetFriends());
        var toMeRequestResult =
            await apiService.GetRequest<List<Friend>>(URL.GetRequests((int)DirectionRequestEnum.Incoming));
        
        if(!friendsResult.IsSuccess && friendsResult.ErrorMessage != null)
            await dialogService.ShowMessage(Localization.Error, friendsResult.ErrorMessage);
        else if(!toMeRequestResult.IsSuccess && toMeRequestResult.ErrorMessage != null)
            await dialogService.ShowMessage(Localization.Error, toMeRequestResult.ErrorMessage);
        else if (friendsResult.IsSuccess && friendsResult.Data != null && 
                 toMeRequestResult.IsSuccess && toMeRequestResult.Data != null)
        {
            Friends = new ObservableCollection<Friend>(friendsResult.Data);
            Requests = new ObservableCollection<Friend>(toMeRequestResult.Data);
        }
        else
            await dialogService.ShowMessage(Localization.Error, Localization.UnexpectedError);
    }
}