using CommunityToolkit.Maui.Storage;

#if ANDROID
using Android.Provider;
#endif

namespace MemoryTrave.Maui.Services.SavePhoto;

public class PhotoSaveService : IPhotoSaveService
{
    public async Task DownloadPhotoAsync(List<string> photoPaths)
    {
        if (photoPaths.Count <= 0)
            return;

        if (DeviceInfo.Current.Platform == DevicePlatform.WinUI ||
            DeviceInfo.Current.Platform == DevicePlatform.MacCatalyst)
        {
            var folderPickerResult = await FolderPicker.Default.PickAsync(CancellationToken.None);
            
            if (!folderPickerResult.IsSuccessful || folderPickerResult.Folder == null)
                return;
            
            var targetFolderPath = folderPickerResult.Folder.Path;
            
            for (var i = 0; i < photoPaths.Count; i++)
            {
                if (File.Exists(photoPaths[i]))
                {
                    var extension = Path.GetExtension(photoPaths[i]);
                    var fileName = $"photo_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}_{i+1}{extension}";
                    
                    var destination = Path.Combine(targetFolderPath, fileName);
                    File.Copy(photoPaths[i], destination, overwrite: true);
                }
            }
        }
        else
        {
            for (var i = 0; i < photoPaths.Count; i++)
            {
                if (File.Exists(photoPaths[i]))
                {
                    var extension = Path.GetExtension(photoPaths[i]);
                    var fileName = $"photo_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}_{i+1}{extension}";
                    await SaveToMobileGalleryAsync(photoPaths[i], fileName);
                }
            }
        }
    }

    private async Task SaveToMobileGalleryAsync(string path, string fileName)
    {
#if ANDROID
        var context = Platform.CurrentActivity;
        var contentValues = new Android.Content.ContentValues();
        contentValues.Put(MediaStore.IMediaColumns.DisplayName, fileName);
        contentValues.Put(MediaStore.IMediaColumns.MimeType, "image/jpeg");
        contentValues.Put(MediaStore.IMediaColumns.RelativePath, Android.OS.Environment.DirectoryPictures);

        var uri = context.ContentResolver.Insert(MediaStore.Images.Media.ExternalContentUri, contentValues);
        if (uri == null) throw new Exception("Не удалось создать запись в MediaStore Android");

        using (var stream = context.ContentResolver.OpenOutputStream(uri))
        using (var fileStream = File.OpenRead(path))
        {
            await fileStream.CopyToAsync(stream);
        }
#elif IOS
        var image = UIKit.UIImage.FromFile(path);
        if (image == null) throw new Exception("Не удалось загрузить изображение из файла для iOS");

        var tcs = new TaskCompletionSource<bool>();
        
        Photos.PHPhotoLibrary.SharedPhotoLibrary.PerformChanges(() => {
            Photos.PHAssetChangeRequest.FromImage(image);
        }, (success, error) => {
            if (success) tcs.SetResult(true);
            else tcs.SetException(new Exception(error?.LocalizedDescription ?? "Ошибка iOS PhotoLibrary"));
        });

        await tcs.Task;
#endif
        await Task.CompletedTask;
    }
}