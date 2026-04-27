using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MemoryTrave.Maui.Infrastructure.Api;
using MemoryTrave.Maui.Models;
using MemoryTrave.Maui.Models.Friends;
using MemoryTrave.Maui.Resources.Localization;
using MemoryTrave.Maui.Services.Dialog;

namespace MemoryTrave.Maui.ViewModel;

public partial class FindFriendsViewModel(
    ApiRequestService apiService,
    IDialogService dialogService) : ObservableObject
{
    private List<User> _allUsers = [];
    
    [ObservableProperty]
    private ObservableCollection<User> _users = [];
    
    [ObservableProperty]
    private string _searchQuery = string.Empty;

    public async Task LoadUsersAsync()
    {
        var response = await apiService.GetRequest<List<User>>(URL.GetUsersWithoutMe());
        if (response.IsSuccess && response.Data != null)
        {
            _allUsers = response.Data;
            Users = new ObservableCollection<User>(_allUsers);
        }
        else if (!response.IsSuccess && response.ErrorMessage != null)
            await dialogService.ShowMessage(Localization.Error, response.ErrorMessage);
        else
            await dialogService.ShowMessage(Localization.Error, Localization.UnexpectedError);
    }

    [RelayCommand]
    private async Task AddFriendAsync(Guid userId)
    {
        var request = new GetId();
        request.Id = userId;

        var response = await apiService.PostRequest<GetId, bool>(URL.AddRequest(), request);
        if (response.IsSuccess)
        {
            var userToRemove = Users.FirstOrDefault(u => u.Id == userId);
            if (userToRemove != null)
            {
                Users.Remove(userToRemove);
            }
        }
        else if (!response.IsSuccess && response.ErrorMessage != null)
            await dialogService.ShowMessage(Localization.Error, response.ErrorMessage);
        else
            await dialogService.ShowMessage(Localization.Error, Localization.UnexpectedError);
    }
    
    [RelayCommand]
    private void SearchUsers()
    {
        if(string.IsNullOrWhiteSpace(SearchQuery))
        {
            Users = new ObservableCollection<User>(_allUsers);
            return;
        }
        
        var findingUsers = _allUsers.Where(u => u.Email.Contains(SearchQuery) 
                                                || u.Name.Contains(SearchQuery));
        Users = new ObservableCollection<User>(findingUsers);
    }
}