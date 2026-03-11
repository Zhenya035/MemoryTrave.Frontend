using MemoryTrave.Maui.ViewModel;

namespace MemoryTrave.Maui.View;

public partial class FriendsPage : ContentPage
{
    private readonly FriendsViewModel _viewModel;
    public FriendsPage(FriendsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.GetFriendsAsync();
    }
}