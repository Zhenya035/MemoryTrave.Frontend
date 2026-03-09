using MemoryTrave.Maui.ViewModel;

namespace MemoryTrave.Maui.View;

public partial class AppShell : Shell
{
    public AppShell(AppShellViewModel vm)
    {
        BindingContext = vm;
        InitializeComponent();

        Routing.RegisterRoute(nameof(AuthPage), typeof(AuthPage));
        Routing.RegisterRoute(nameof(AddArticlePage), typeof(AddArticlePage));
        Routing.RegisterRoute(nameof(ArticleDetailPage), typeof(ArticleDetailPage));
        Routing.RegisterRoute(nameof(FriendsPage), typeof(FriendsPage));
    }
}