using CommunityToolkit.Mvvm.ComponentModel;
using MemoryTrave.Maui.Services.Interfaces;

namespace MemoryTrave.Maui.ViewModel;

public class BaseViewModel : ObservableObject
{
    protected readonly ILocalizationService _localizationService;
    
    protected BaseViewModel(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
        
        _localizationService.CultureChanged += OnCultureChanged;
    }

    protected virtual void OnCultureChanged() =>
        OnPropertyChanged(string.Empty);

    ~BaseViewModel() =>
        _localizationService.CultureChanged -= OnCultureChanged;
}