using MemoryTrave.Maui.ViewModel;

namespace MemoryTrave.Maui.View;

[QueryProperty(nameof(LocationId), "id")]
public partial class LocationDetailPage : ContentPage
{
    private readonly LocationDetailViewModel _viewModel;
    private string _locationId;
    public string LocationId
    {
        get => _locationId;
        set
        {
            _locationId = value;
        }
    }

    public LocationDetailPage(LocationDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (!string.IsNullOrEmpty(_locationId))
        {
            await _viewModel.LoadLocationAsync(_locationId);
            
            Title = _viewModel.LocationName;
        }
    }
}