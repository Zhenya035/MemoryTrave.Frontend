using MemoryTrave.Maui.ViewModel;

namespace MemoryTrave.Maui.View;

public partial class LocationDetailPage : ContentPage
{
    public LocationDetailPage(LocationDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}