using Mapsui;
using Mapsui.Layers;
using Mapsui.UI.Maui;
using MemoryTrave.Maui.ViewModel;
using MemoryTrave.Maui.Models.Location;
using Location = MemoryTrave.Maui.Models.Location.Location;

namespace MemoryTrave.Maui.View;

public partial class MapPage : ContentPage
{
    private readonly MapViewModel _viewModel;

    public MapPage(MapViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;

        MapControl.Info += OnMapInfo;
    }

    private async void OnMapInfo(object? sender, MapInfoEventArgs e)
    {
        var mapInfo = e.GetMapInfo(MapControl.Map.Layers);
        
        if (mapInfo.Feature?["Location"] is Location location)
        {
            var parameters = new Dictionary<string, object>
            {
                ["id"] = location.Id.ToString()
            };
            await Shell.Current.GoToAsync(nameof(LocationDetailPage), parameters);
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.GetLocationsAsync();
    }

    protected override void OnDisappearing()
    {
        MapControl.Info -= OnMapInfo;
        base.OnDisappearing();
    }
}