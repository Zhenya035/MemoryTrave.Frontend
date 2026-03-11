using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MemoryTrave.Maui.Infrastructure.Api;
using MemoryTrave.Maui.Models.Friends;
using MemoryTrave.Maui.Resources.Localization;
using MemoryTrave.Maui.Services.Interfaces;

namespace MemoryTrave.Maui.ViewModel;

public partial class FriendsViewModel(
    ApiRequestService apiService,
    IDialogService dialogService) : ObservableObject
{
    [ObservableProperty]
    private List<Friend> _friends = [];

    [ObservableProperty]
    private List<Friend> _requests = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(InRequests))]
    private bool _inFriendship;

    public bool InRequests => !InFriendship;
    
    [RelayCommand]
    private void RequestsTapped() =>
        InFriendship = false;
    
    [RelayCommand]
    private void FriendshipTapped() =>
        InFriendship = true;

    public async Task GetFriendsAsync()
    {
        var friendsResult = await apiService.GetRequest<List<Friend>>(URL.GetFriends());
        var requestResult = await apiService.GetRequest<List<Friend>>(URL.GetRequests());
        
        if(!friendsResult.IsSuccess && friendsResult.ErrorMessage != null)
            await dialogService.ShowMessage(Localization.Error, friendsResult.ErrorMessage);
        else if(!requestResult.IsSuccess && requestResult.ErrorMessage != null)
            await dialogService.ShowMessage(Localization.Error, requestResult.ErrorMessage);
        else if (friendsResult.IsSuccess && friendsResult.Data != null && 
                 requestResult.IsSuccess && requestResult.Data != null)
        {
            Friends = friendsResult.Data;
            Requests = requestResult.Data;
        }
        else
            await dialogService.ShowMessage(Localization.Error, Localization.UnexpectedError);
    }
}