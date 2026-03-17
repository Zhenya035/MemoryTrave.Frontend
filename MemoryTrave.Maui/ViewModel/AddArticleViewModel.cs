using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MemoryTrave.Maui.Infrastructure.Api;
using MemoryTrave.Maui.Models;
using MemoryTrave.Maui.Models.Articles;
using MemoryTrave.Maui.Models.Photos;
using MemoryTrave.Maui.Resources.Localization;
using MemoryTrave.Maui.Services.Dialog;
using MemoryTrave.Maui.Services.Navigation;

namespace MemoryTrave.Maui.ViewModel;

[QueryProperty(nameof(LocationId), "id")]
public partial class AddArticleViewModel(
    IDialogService dialogService,
    INavigationService navigation,
    ApiRequestService apiService) : ObservableObject
{
    [ObservableProperty]
    private string _locationId = string.Empty;
    
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
                var addArticleRequest = new AddPublicArticle()
                {
                    Description = Description,
                    LocationId = LocationId
                };
                
                var addArticleResponse = await apiService.PostRequest<AddPublicArticle, GetId>
                    (URL.AddPublicArticle(), addArticleRequest);
                if(!addArticleResponse.IsSuccess && addArticleResponse.ErrorMessage != null)
                {
                    await dialogService.ShowMessage(Localization.Error, addArticleResponse.ErrorMessage);
                    return;
                }

                var articleId = addArticleResponse.Data.Id;
                
                var addPhotoRequest = new PhotoList();
                foreach (var path in Photos)
                {
                    if (!File.Exists(path)) continue;
                    var photoBytes = await File.ReadAllBytesAsync(path);
                    var photoString = Convert.ToBase64String(photoBytes);
                    addPhotoRequest.Photos.Add(photoString);
                }
                
                var addPhotoResponse = await apiService.PostRequest<PhotoList, PhotoList>
                    (URL.UploadPhoto(articleId.ToString()), addPhotoRequest);
                
                if (!addPhotoResponse.IsSuccess && addPhotoResponse.ErrorMessage != null)
                    await dialogService.ShowMessage(Localization.Error, addPhotoResponse.ErrorMessage);
                
                var addPhotoToArticleResponse = await apiService.PostRequest<PhotoList, bool>
                    (URL.AddPhotoToPublicArticle(articleId.ToString()), addPhotoResponse.Data);
                
                if (!addPhotoToArticleResponse.IsSuccess && addPhotoToArticleResponse.ErrorMessage != null)
                    await dialogService.ShowMessage(Localization.Error, addPhotoToArticleResponse.ErrorMessage);
            }
            else if (SelectedVisibility == Localization.FriendVisibility)
            {

            }
            else if (SelectedVisibility == Localization.PrivateVisibility)
            {

            }

            await navigation.GoBack();
        }
        catch
        {
            await dialogService.ShowMessage(Localization.Error, Localization.UnexpectedError);
        }
    }
}