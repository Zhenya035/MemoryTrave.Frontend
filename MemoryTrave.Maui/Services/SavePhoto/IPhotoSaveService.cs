namespace MemoryTrave.Maui.Services.SavePhoto;

public interface IPhotoSaveService
{
    public Task DownloadPhotoAsync(List<string> photoPaths);
}