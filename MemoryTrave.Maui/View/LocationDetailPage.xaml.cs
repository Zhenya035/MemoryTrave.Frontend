using MemoryTrave.Maui.ViewModel;

namespace MemoryTrave.Maui.View;

public partial class LocationDetailPage : ContentPage
{
    private LocationDetailViewModel? _viewModel;
    public LocationDetailPage(LocationDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        
        _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.GetLocationAsync();
    }
}