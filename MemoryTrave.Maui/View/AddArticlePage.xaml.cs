using MemoryTrave.Maui.ViewModel;

namespace MemoryTrave.Maui.View;

public partial class AddArticlePage : ContentPage
{
    private readonly AddArticleViewModel _viewModel;
    
    public AddArticlePage(AddArticleViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        
        _viewModel = viewModel;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        
        _viewModel.ClearCache();
    }
}