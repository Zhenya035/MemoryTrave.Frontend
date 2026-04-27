using SkiaSharp;

namespace MemoryTrave.Maui.Services.Photo;

public class PhotoService : IPhotoService
{
    private const int MaxImageDimension = 1200;
    private const int JpegQuality = 75;
    
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

            try
            {
                var photoBytes = Convert.FromBase64String(photo);
                
                using var bitmap = SKBitmap.Decode(photoBytes);
                if(bitmap == null)
                    continue;
                
                var ratio = Math.Min((float)MaxImageDimension / bitmap.Width, (float)MaxImageDimension / bitmap.Height);

                using var finalBitmap = ratio < 1.0f 
                    ? bitmap.Resize(new SKImageInfo((int)(bitmap.Width * ratio), (int)(bitmap.Height * ratio)), SKFilterQuality.Medium) 
                    : null;
                
                var bitmapToSave = finalBitmap ?? bitmap;
                
                var fileName = $"{Guid.NewGuid()}.jpg";
                var localPath = Path.Combine(FileSystem.CacheDirectory, fileName);
                
                using (var image = SKImage.FromBitmap(bitmapToSave))
                using (var data = image.Encode(SKEncodedImageFormat.Jpeg, JpegQuality))
                await using (var stream = File.OpenWrite(localPath))
                {
                    data.SaveTo(stream);
                }

                result.Add(localPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
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