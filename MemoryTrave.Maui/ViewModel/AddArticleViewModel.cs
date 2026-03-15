using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MemoryTrave.Maui.Resources.Localization;
using MemoryTrave.Maui.Services.Dialog;

namespace MemoryTrave.Maui.ViewModel;

public partial class AddArticleViewModel(IDialogService dialogService) : ObservableObject
{
    [ObservableProperty]
    private string _locationName = string.Empty;
    
    [ObservableProperty]
    private string _description = string.Empty;
    
    [ObservableProperty]
    private ObservableCollection<string> _photos = [];
    
    public ObservableCollection<string> VisibilityLevels { get; } =
    [
        Localization.PublicVisibility,
        Localization.FriendVisibility,
        Localization.PrivateVisibility
    ];
    
    [ObservableProperty]
    private string _selectedVisibility = Localization.PublicVisibility;
    
    [ObservableProperty]
    private bool _isBusy;

    [RelayCommand]
    [Obsolete("Obsolete")]
    private async Task AddPhoto()
    {
        if (IsBusy) return;
        
        try
        {
            IsBusy = true;
            
            var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Выберите фото"
            });

            if (result != null)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(result.FileName)}";
                var localPath = Path.Combine(FileSystem.CacheDirectory, fileName);

                await using var stream = await result.OpenReadAsync();
                await using var localStream = File.OpenWrite(localPath);
                await stream.CopyToAsync(localStream);

                Photos.Add(localPath);
            }
        }
        catch (Exception ex)
        {
            await dialogService.ShowMessage(Localization.Error, ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void RemovePhoto(string photoPath)
    {
        if (string.IsNullOrEmpty(photoPath) || !Photos.Contains(photoPath)) return;
        Photos.Remove(photoPath);

        try
        {
            if (File.Exists(photoPath))
                File.Delete(photoPath);
        }
        catch
        {
            dialogService.ShowMessage(Localization.Error, Localization.UnexpectedError);
        }
    }

    [RelayCommand]
    private async Task Save()
    {
        try
        {
            
            
            if (SelectedVisibility == Localization.PublicVisibility)
            {
                
            }
            else if (SelectedVisibility == Localization.FriendVisibility)
            {

            }
            else if (SelectedVisibility == Localization.PrivateVisibility)
            {

            }
        }
        catch
        {
            await dialogService.ShowMessage(Localization.Error, Localization.UnexpectedError);
        }
    }
}