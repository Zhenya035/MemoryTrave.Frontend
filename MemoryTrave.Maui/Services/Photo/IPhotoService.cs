namespace MemoryTrave.Maui.Services.Photo;

public interface IPhotoService
{
    public Task<string> AddPhotoToLocalFromMediaPickerAsync(FileResult result);
    public Task<List<string>> AddPhotosToLocalAsync(List<string> photos, string articleId);
    
    public Task<List<string>> GetPhotoFromLocalAsync(List<string> photos);
   
    public void RemovePhotoFromLocal(string photo);
    public void RemovePhotosFromLocal(List<string> photos);
    
    public void ClearCacheForArticle(string articleId);
}