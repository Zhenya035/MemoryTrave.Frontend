using MemoryTrave.Maui.ViewModel;

namespace MemoryTrave.Maui.View;

public partial class ProfilePage : ContentPage
{
    private readonly ProfileViewModel _viewModel;
    
    public ProfilePage(ProfileViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.GetProfileAsync();
    }
}