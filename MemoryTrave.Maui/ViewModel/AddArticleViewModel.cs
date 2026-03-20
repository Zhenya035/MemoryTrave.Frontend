using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MemoryTrave.Maui.Infrastructure.Api;
using MemoryTrave.Maui.Infrastructure.Security;
using MemoryTrave.Maui.Models;
using MemoryTrave.Maui.Models.Articles;
using MemoryTrave.Maui.Models.Articles.Access;
using MemoryTrave.Maui.Models.Photos;
using MemoryTrave.Maui.Resources.Localization;
using MemoryTrave.Maui.Services.Dialog;
using MemoryTrave.Maui.Services.Navigation;
using MemoryTrave.Maui.Services.Photo;

namespace MemoryTrave.Maui.ViewModel;

[QueryProperty(nameof(LocationId), "id")]
public partial class AddArticleViewModel(
    IDialogService dialogService,
    INavigationService navigation,
    IPhotoService photoService,
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
                var localPath = await photoService.AddPhotoToLocalFromMediaPickerAsync(result);
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
            photoService.RemovePhotoFromLocal(photoPath);
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
                addPhotoRequest.Photos = await photoService.GetPhotoFromLocalAsync(Photos.ToList());
                
                var addPhotoResponse = await apiService.PostRequest<PhotoList, PhotoList>
                    (URL.UploadPhoto(articleId.ToString()), addPhotoRequest);
                
                if (!addPhotoResponse.IsSuccess && addPhotoResponse.ErrorMessage != null)
                    await dialogService.ShowMessage(Localization.Error, addPhotoResponse.ErrorMessage);
            }
            else
            {
                var addArticleResponse = await apiService.PostRequest<GetId>
                    (URL.AddPrivateArticle(LocationId));
                if(!addArticleResponse.IsSuccess && addArticleResponse.ErrorMessage != null)
                {
                    await dialogService.ShowMessage(Localization.Error, addArticleResponse.ErrorMessage);
                    return;
                }

                var articleId = addArticleResponse.Data.Id;
                
                var dek = AesGcm256.GenerateKey();
                var photos = await photoService.GetPhotoFromLocalAsync(Photos.ToList());
                
                var addPhotoRequest = new PhotoList();
                addPhotoRequest.Photos = photos.Select(photo => AesGcm256.Encrypt(photo, dek)).ToList();
                
                var addPhotoResponse = await apiService.PostRequest<PhotoList, PhotoList>
                    (URL.UploadPhoto(articleId.ToString()), addPhotoRequest);
                
                if (!addPhotoResponse.IsSuccess && addPhotoResponse.ErrorMessage != null)
                    await dialogService.ShowMessage(Localization.Error, addPhotoResponse.ErrorMessage);

                var description = new PrivateArticle
                {
                    Description = Description
                };
                var json = JsonSerializer.Serialize(description);
                
                var encryptedPrivate = AesGcm256.Encrypt(json, dek);

                var myPublicKeyResult = await apiService.GetRequest<GetPublicKey>(URL.GetPublicKey());
                if (!myPublicKeyResult.IsSuccess && myPublicKeyResult.ErrorMessage != null)
                {
                    await dialogService.ShowMessage(Localization.Error, myPublicKeyResult.ErrorMessage);
                    return;
                }

                var acess = new List<AddEncryptedKeys>();
                var encryptedKey = GetEncryptedKeys(myPublicKeyResult.Data.PublicKey, myPublicKeyResult.Data.UserId, dek);
                acess.Add(encryptedKey);

                if (SelectedVisibility == Localization.FriendVisibility)
                {
                    var friendsPublicKeyResult =
                        await apiService.GetRequest<List<GetPublicKey>>(URL.GetFriendsPublicKeys());
                    if (!friendsPublicKeyResult.IsSuccess && friendsPublicKeyResult.ErrorMessage != null)
                    {
                        await dialogService.ShowMessage(Localization.Error, friendsPublicKeyResult.ErrorMessage);
                        return;
                    }
                    
                    foreach (var key in friendsPublicKeyResult.Data)
                    {
                        encryptedKey = GetEncryptedKeys(key.PublicKey, key.UserId, dek);
                        acess.Add(encryptedKey);
                    }
                }

                var addRequest = new AddPrivateArticleData
                {
                    EncryptedDescription = encryptedPrivate,
                    EncryptedKeys = acess
                };

                var result = await apiService.PostRequest<AddPrivateArticleData, bool>
                    (URL.AddDataToPrivateArticle(articleId.ToString()), addRequest);

                if (!result.IsSuccess && result.ErrorMessage != null)
                    await dialogService.ShowMessage(Localization.Error, result.ErrorMessage);
            }

            await navigation.GoBack();
        }
        catch
        {
            await dialogService.ShowMessage(Localization.Error, Localization.UnexpectedError);
        }
    }

    private static AddEncryptedKeys GetEncryptedKeys(string publicKeyString, Guid userId, byte[] dek)
    {
        var publicKey = EccP256.StringToPublicKey(publicKeyString);

        var encryptedKeyBytes = EccP256.Encrypt(publicKey, dek);
        var encryptedKey = Convert.ToBase64String(encryptedKeyBytes);

        return new AddEncryptedKeys { EncryptedKey = encryptedKey, UserId = userId };
    }
    
    public void ClearCache()
    {
        if(Photos.Count == 0)
            return;
        photoService.RemovePhotosFromLocal(Photos.ToList());
    }
}