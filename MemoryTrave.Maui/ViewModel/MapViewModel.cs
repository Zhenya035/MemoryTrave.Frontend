using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mapsui;
using Mapsui.Projections;
using Mapsui.Tiling;
using MemoryTrave.Maui.Services.Interfaces;
using MemoryTrave.Maui.View;
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
    
    public MapViewModel(INavigationService navigation, IAuthService authService)
    {
        Map = new Map();
        Map.Layers.Add(OpenStreetMap.CreateTileLayer());
        
        var startLocation = SphericalMercator.FromLonLat(27.953389, 53.709807);
    
        Map.Navigator.CenterOn(startLocation.x, startLocation.y);
        Map.Navigator.ZoomToLevel(6);

        Map.Navigator.OverrideZoomBounds = new MMinMax(5, 10000);
        
        Map.Refresh();
        
        _authService = authService;
        _navigation = navigation;
        
        _authService.AuthStateChanged += OnAuthStateChanged;
        
        UpdateLoginButtonVisibility();
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