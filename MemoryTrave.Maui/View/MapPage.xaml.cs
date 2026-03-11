using MemoryTrave.Maui.ViewModel;

namespace MemoryTrave.Maui.View;

public partial class MapPage : ContentPage
{
    public MapPage(MapViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}