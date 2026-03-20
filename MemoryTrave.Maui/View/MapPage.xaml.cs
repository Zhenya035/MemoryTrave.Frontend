using Mapsui;
using MemoryTrave.Maui.ViewModel;
using Location = MemoryTrave.Maui.Models.Location.Location;

namespace MemoryTrave.Maui.View;

public partial class MapPage : ContentPage
{
    private readonly MapViewModel _viewModel;

    public MapPage(MapViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    private async void OnMapInfo(object? sender, MapInfoEventArgs e)
    {
        var mapInfo = e.GetMapInfo(MapControl.Map.Layers);
    
        if (mapInfo?.Layer?.Name != "UserLocations")
            return;
        
        if (mapInfo.Feature?["Id"] is string id && !string.IsNullOrEmpty(id))
        {
            var parameters = new Dictionary<string, object>
            {
                ["id"] = id
            };
        
            await Shell.Current.GoToAsync(nameof(LocationDetailPage), parameters);
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        MapControl.Info += OnMapInfo;
        await _viewModel.GetLocationsAsync();
    }

    protected override void OnDisappearing()
    {
        MapControl.Info -= OnMapInfo;
        base.OnDisappearing();
    }
}