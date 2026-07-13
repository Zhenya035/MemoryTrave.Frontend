using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Tiling;
using MemoryTrave.Maui.Infrastructure.Api;
using MemoryTrave.Maui.Models.Enums;
using MemoryTrave.Maui.Resources.Localization;
using MemoryTrave.Maui.Services.Auth;
using MemoryTrave.Maui.Services.Dialog;
using MemoryTrave.Maui.Services.Navigation;
using MemoryTrave.Maui.View;
using Location = MemoryTrave.Maui.Models.Location.Location;
using Map = Mapsui.Map;
using Color = Mapsui.Styles.Color;
using Point = NetTopologySuite.Geometries.Point;
using Brush = Mapsui.Styles.Brush;

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
    private List<(Point Point, string Id, string Color)> _allPoints;

    private const double ClusterResolutionThreshold = 20;

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
        Map.Navigator.OverrideZoomBounds = new MMinMax(3, 10000);

        _authService = authService;
        _dialogService = dialogService;
        _apiService = apiService;
        _navigation = navigation;

        _authService.AuthStateChanged += OnAuthStateChanged;
        UpdateLoginButtonVisibility();

        Map.Navigator.ViewportChanged += (s, e) =>
        {
            if (_allPoints != null && _allPoints.Count > 0)
                UpdateLayerByZoom(e.Viewport.Resolution);
        };
        Map.Refresh();
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
            DisplayLocationsOnMap(result.Data);
        else
            await _dialogService.ShowMessage(Localization.Error, Localization.UnexpectedError);
    }

    private void DisplayLocationsOnMap(IEnumerable<Location> locations)
    {
        _allPoints = locations.Select(loc =>
        {
            var (x, y) = SphericalMercator.FromLonLat(loc.Longitude, loc.Latitude);
            var color = loc.LocationContentState switch
            {
                LocationContentState.Empty => "#969696",
                LocationContentState.MyArticles => "#9C27B0",
                LocationContentState.FriendsArticles => "#34A853",
                LocationContentState.MyAndFriendsArticles => "#EA4335",
                _ => "#FBBC04"
            };
            return (new Point(x, y), loc.Id.ToString(), color);
        }).ToList();

        if (_locationsLayer == null)
        {
            _locationsLayer = new MemoryLayer { Name = "UserLocations" };
            Map.Layers.Add(_locationsLayer);
        }

        UpdateLayerByZoom(Map.Navigator.Viewport.Resolution);
    }

    private void UpdateLayerByZoom(double resolution)
    {
        if (_allPoints == null || _locationsLayer == null) return;

        if (resolution > ClusterResolutionThreshold)
        {
            _locationsLayer.Features = GenerateClusterFeatures();
        }
        else
        {
            _locationsLayer.Features = GeneratePointFeatures();
        }

        Map.Refresh();
    }

    private List<IFeature> GeneratePointFeatures()
    {
        return _allPoints.Select(p =>
        {
            var feature = new Mapsui.Nts.GeometryFeature { Geometry = p.Point };
            feature["Id"] = p.Id;

            var symbolStyle = new SymbolStyle
            {
                SymbolScale = 0.8,
                Fill = new Brush { Color = Color.FromString(p.Color) },
                Outline = new Pen { Color = Color.White, Width = 2 }
            };

            feature.Styles.Add(symbolStyle);
            return (IFeature)feature;
        }).ToList();
    }

    private List<IFeature> GenerateClusterFeatures()
    {
        var resolution = Map.Navigator.Viewport.Resolution;
        double maxDist = ClusterResolutionThreshold * resolution * 0.2;
        var clusters = SimpleCluster(_allPoints, maxDist);
        var features = new List<IFeature>();

        foreach (var cluster in clusters)
        {
            if (cluster.Count == 1)
            {
                var singlePoint = cluster[0];
                var pointFeature = new Mapsui.Nts.GeometryFeature { Geometry = singlePoint.Point };
                pointFeature["Id"] = singlePoint.Id;

                var symbol = new SymbolStyle
                {
                    SymbolScale = 0.8,
                    Fill = new Brush { Color = Color.FromString(singlePoint.Color) },
                    Outline = new Pen { Color = Color.White, Width = 2 }
                };
                pointFeature.Styles.Add(symbol);
                features.Add(pointFeature);
            }
            else
            {
                var centerX = cluster.Average(p => p.Point.X);
                var centerY = cluster.Average(p => p.Point.Y);
                var center = new Point(centerX, centerY);
                int count = cluster.Count;

                var clusterFeature = new Mapsui.Nts.GeometryFeature { Geometry = center };
                clusterFeature["Count"] = count;

                var clusterSymbol = new SymbolStyle
                {
                    SymbolScale = 1.0 + Math.Min(count, 50) * 0.02,
                    Fill = new Brush { Color = new Color(66, 133, 244, 180) },
                    Outline = new Pen { Color = Color.White, Width = 2 }
                };

                var label = new LabelStyle
                {
                    Text = count.ToString(),
                    ForeColor = Color.White,
                    HorizontalAlignment = LabelStyle.HorizontalAlignmentEnum.Center,
                    VerticalAlignment = LabelStyle.VerticalAlignmentEnum.Center,
                    BackColor = null,
                    Offset = new Offset(0, 0)
                };

                clusterFeature.Styles.Add(clusterSymbol);
                clusterFeature.Styles.Add(label);
                features.Add(clusterFeature);
            }
        }

        return features;
    }

    private List<List<(Point Point, string Id, string Color)>> SimpleCluster(
        List<(Point Point, string Id, string Color)> points, double maxDist)
    {
        var clusters = new List<List<(Point Point, string Id, string Color)>>();
        var used = new bool[points.Count];

        for (int i = 0; i < points.Count; i++)
        {
            if (used[i]) continue;
            var cluster = new List<(Point Point, string Id, string Color)> { points[i] };
            used[i] = true;

            for (int j = i + 1; j < points.Count; j++)
            {
                if (!used[j] && Distance(points[i].Point, points[j].Point) < maxDist)
                {
                    cluster.Add(points[j]);
                    used[j] = true;
                }
            }
            clusters.Add(cluster);
        }

        return clusters;
    }

    private double Distance(Point a, Point b)
    {
        var dx = a.X - b.X;
        var dy = a.Y - b.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    private void OnAuthStateChanged() => UpdateLoginButtonVisibility();
    private void UpdateLoginButtonVisibility()
    {
        MainThread.BeginInvokeOnMainThread(() => IsAuthButVisible = !_authService.IsAuthorized);
    }
}