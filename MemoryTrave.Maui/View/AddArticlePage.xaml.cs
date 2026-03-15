using MemoryTrave.Maui.ViewModel;

namespace MemoryTrave.Maui.View;

public partial class AddArticlePage : ContentPage
{
    public AddArticlePage(AddArticleViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}