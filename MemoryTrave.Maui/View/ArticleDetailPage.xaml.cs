using MemoryTrave.Maui.ViewModel;

namespace MemoryTrave.Maui.View;

public partial class ArticleDetailPage : ContentPage
{
    public ArticleDetailPage(ArticleDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}