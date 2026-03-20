using MemoryTrave.Maui.ViewModel;

namespace MemoryTrave.Maui.View;

public partial class AuthPage : ContentPage
{
    public AuthPage(AuthViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}