namespace MemoryTrave.Maui.Services.Photo;

public class PhotoService : IPhotoService
{
    public async Task<string> AddPhotoToLocalFromMediaPickerAsync(FileResult result)
    {
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(result.FileName)}";
        var localPath = Path.Combine(FileSystem.CacheDirectory, fileName);

        await using var stream = await result.OpenReadAsync();
        await using var localStream = File.OpenWrite(localPath);
        await stream.CopyToAsync(localStream);
        
        return localPath;
    }

    public async Task<List<string>> AddPhotosToLocalAsync(List<string> photos)
    {
        var result = new List<string>();
        foreach (var photo in photos)
        {
            if (string.IsNullOrEmpty(photo))
                continue;

            var photoBytes = Convert.FromBase64String(photo);
            
            var fileName = $"{Guid.NewGuid()}.jpg";
            var localPath = Path.Combine(FileSystem.CacheDirectory, fileName);
            
            await File.WriteAllBytesAsync(localPath, photoBytes);
            
            result.Add(localPath);
        }
        return result;
    }

    public async Task<string> AddPhotoToLocalAsync(string photo)
    {
        if (string.IsNullOrEmpty(photo))
            return string.Empty;

        var photoBytes = Convert.FromBase64String(photo);
            
        var fileName = $"{Guid.NewGuid()}.jpg";
        var localPath = Path.Combine(FileSystem.CacheDirectory, fileName);
            
        await File.WriteAllBytesAsync(localPath, photoBytes);
        
        return localPath;
    }

    public async Task<List<string>> GetPhotoFromLocalAsync(List<string> photos)
    {
        var result = new List<string>();
        
        foreach (var path in photos)
        {
            if (!File.Exists(path)) continue;
            var photoBytes = await File.ReadAllBytesAsync(path);
            var photoString = Convert.ToBase64String(photoBytes);
            result.Add(photoString);
        }
        
        return result;
    }

    public void RemovePhotoFromLocal(string photo)
    {
        if (File.Exists(photo))
            File.Delete(photo);
    }

    public void RemovePhotosFromLocal(List<string> photos)
    {
        foreach (var photo in photos)
        {
            if (File.Exists(photo))
                File.Delete(photo);
        }
    }
}