using MemoryTrave.Maui.ViewModel;

namespace MemoryTrave.Maui.View;

public partial class ArticleDetailPage : ContentPage
{
    private readonly ArticleDetailViewModel _viewModel;
    
    public ArticleDetailPage(ArticleDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        
        _viewModel = viewModel;
    }
}