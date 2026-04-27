using MemoryTrave.Maui.ViewModel;

namespace MemoryTrave.Maui.View;

public partial class FindFriendsPage : ContentPage
{
    private FindFriendsViewModel _viewModel;
    
    public FindFriendsPage(FindFriendsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        
        _viewModel = vm;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadUsersAsync();
    }
}