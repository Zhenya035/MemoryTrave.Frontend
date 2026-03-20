using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Tiling;
using MemoryTrave.Maui.Infrastructure.Api;
using MemoryTrave.Maui.Resources.Localization;
using MemoryTrave.Maui.Services.Auth;
using MemoryTrave.Maui.Services.Dialog;
using MemoryTrave.Maui.Services.Navigation;
using MemoryTrave.Maui.View;
using Brush = Mapsui.Styles.Brush;
using Color = Mapsui.Styles.Color;
using Location = MemoryTrave.Maui.Models.Location.Location;
using Map = Mapsui.Map;

namespace MemoryTrave.Maui.ViewModel;

public partial class MapViewModel : ObservableObject
{
    [ObservableProperty]
    private Map _map;
    
    [ObservableProperty]
    private bool _isAuthButVisible = true;
    
    [RelayCommand]
    private async Task OnLoginTapped()
    {
        await _navigation.GoTo(nameof(AuthPage));
    }
    
    private readonly INavigationService _navigation;
    private readonly IAuthService _authService;
    private readonly IDialogService _dialogService;
    private readonly ApiRequestService _apiService;

    private MemoryLayer _locationsLayer;

    public MapViewModel(
        INavigationService navigation, 
        IAuthService authService,
        IDialogService dialogService,
        ApiRequestService apiService)
    {
        Map = new Map();
        Map.Layers.Add(OpenStreetMap.CreateTileLayer());
        
        var startLocation = SphericalMercator.FromLonLat(27.953389, 53.709807);
    
        Map.Navigator.CenterOn(startLocation.x, startLocation.y);
        Map.Navigator.ZoomToLevel(6);

        Map.Navigator.OverrideZoomBounds = new MMinMax(5, 10000);
        
        Map.Refresh();
        
        _authService = authService;
        _dialogService = dialogService;
        _apiService = apiService;
        _navigation = navigation;
        
        _authService.AuthStateChanged += OnAuthStateChanged;
        UpdateLoginButtonVisibility();
    }

    public async Task GetLocationsAsync()
    {
        var result = await _apiService.GetRequest<List<Location>>(URL.GetLocations());
        
        if (!result.IsSuccess && result.ErrorMessage != null)
        {
            await _dialogService.ShowMessage(Localization.Error, result.ErrorMessage);
            return;
        }
        
        if (result.IsSuccess && result.Data != null)
        {
            DisplayLocationsOnMap(result.Data);
        }
        else
        {
            await _dialogService.ShowMessage(Localization.Error, Localization.UnexpectedError);
        }
    }

    private void DisplayLocationsOnMap(IEnumerable<Location> locations)
    {
        if (_locationsLayer == null)
        {
            _locationsLayer = new MemoryLayer
            {
                Name = "UserLocations" 
            };
            Map.Layers.Add(_locationsLayer);
        }
        
        var features = new List<IFeature>();
        
        foreach (var loc in locations)
        {
            var mapPoint = SphericalMercator.FromLonLat(loc.Longitude, loc.Latitude);
            
            var feature = new Mapsui.Nts.GeometryFeature
            {
                Geometry = new NetTopologySuite.Geometries.Point(mapPoint.x, mapPoint.y),
                ["Id"] = loc.Id.ToString()
            };
            
            feature.Styles.Add(CreatePinStyle());
            
            features.Add(feature);
        }
        
        _locationsLayer.Features = features;
        
        Map.Refresh();
    }

    private SymbolStyle CreatePinStyle()
    {
        return new SymbolStyle
        {
            SymbolScale = 0.8,
            Fill = new Brush { Color = Color.FromRgba(66, 133, 244, 255) },
            Outline = new Pen { Color = Color.White, Width = 2 }
        };
    }
    
    private void OnAuthStateChanged() => UpdateLoginButtonVisibility();

    private void UpdateLoginButtonVisibility()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            IsAuthButVisible = !_authService.IsAuthorized;
        });
    }
}