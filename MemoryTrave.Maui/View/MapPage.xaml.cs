using Mapsui;
using Mapsui.Tiling;
using MemoryTrave.Maui.ViewModel;
using Map = Mapsui.Map;

namespace MemoryTrave.Maui.View;

public partial class MapPage : ContentPage
{
    private MPoint _point = new MPoint(53.70568405543116, 23.807502623628757);
    
    public MapPage(MapViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;

        var map = new Map();
        map.Layers.Add(OpenStreetMap.CreateTileLayer());
        MapControl.Map = map;
    }
}