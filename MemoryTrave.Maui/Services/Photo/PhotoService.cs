using SkiaSharp;

namespace MemoryTrave.Maui.Services.Photo;

public class PhotoService : IPhotoService
{
    private const int MaxImageDimension = 1200;
    private const int JpegQuality = 75;
    private const string CacheFolder = "article_photos";

    private string GetCacheDirectory()
    {
        var dir = Path.Combine(FileSystem.CacheDirectory, CacheFolder);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        return dir;
    }

    public async Task<string> AddPhotoToLocalFromMediaPickerAsync(FileResult result)
    {
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(result.FileName)}";
        var localPath = Path.Combine(GetCacheDirectory(), fileName);

        await using var stream = await result.OpenReadAsync();
        await using var localStream = File.OpenWrite(localPath);
        await stream.CopyToAsync(localStream);

        return localPath;
    }

    public async Task<List<string>> AddPhotosToLocalAsync(List<string> photos, string articleId)
    {
        CleanupCacheForOtherArticles(articleId);

        var result = new List<string>();
        var cacheDir = GetCacheDirectory();

        foreach (var photo in photos)
        {
            if (string.IsNullOrEmpty(photo)) continue;

            try
            {
                var photoBytes = Convert.FromBase64String(photo);
                using var bitmap = SKBitmap.Decode(photoBytes);
                if (bitmap == null) continue;

                var ratio = Math.Min((float)MaxImageDimension / bitmap.Width, (float)MaxImageDimension / bitmap.Height);

                using var finalBitmap = ratio < 1.0f
                    ? bitmap.Resize(new SKImageInfo((int)(bitmap.Width * ratio), (int)(bitmap.Height * ratio)), SKFilterQuality.Medium)
                    : null;

                var bitmapToSave = finalBitmap ?? bitmap;

                var fileName = $"{articleId}_{Guid.NewGuid()}.jpg";
                var localPath = Path.Combine(cacheDir, fileName);

                using (var image = SKImage.FromBitmap(bitmapToSave))
                using (var data = image.Encode(SKEncodedImageFormat.Jpeg, JpegQuality))
                await using (var stream = File.OpenWrite(localPath))
                {
                    data.SaveTo(stream);
                }

                result.Add(localPath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PhotoService] Error saving photo: {ex.Message}");
            }
        }
        return result;
    }

    public async Task<string> AddPhotoToLocalAsync(string photo, string articleId)
    {
        if (string.IsNullOrEmpty(photo)) return string.Empty;

        CleanupCacheForOtherArticles(articleId);

        var photoBytes = Convert.FromBase64String(photo);
        var fileName = $"{articleId}_{Guid.NewGuid()}.jpg";
        var localPath = Path.Combine(GetCacheDirectory(), fileName);

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
            result.Add(Convert.ToBase64String(photoBytes));
        }
        return result;
    }

    public void RemovePhotoFromLocal(string photoPath)
    {
        if (File.Exists(photoPath))
        {
            try { File.Delete(photoPath); }
            catch { /* игнорируем ошибки удаления */ }
        }
    }

    public void RemovePhotosFromLocal(List<string> photoPaths)
    {
        foreach (var path in photoPaths)
            RemovePhotoFromLocal(path);
    }

    public void ClearCacheForArticle(string articleId)
    {
        var cacheDir = GetCacheDirectory();
        if (!Directory.Exists(cacheDir)) return;

        var files = Directory.GetFiles(cacheDir, $"{articleId}_*.jpg", SearchOption.TopDirectoryOnly);
        foreach (var file in files)
            RemovePhotoFromLocal(file);
    }

    private void CleanupCacheForOtherArticles(string currentArticleId)
    {
        var cacheDir = GetCacheDirectory();
        if (!Directory.Exists(cacheDir)) return;

        var files = Directory.GetFiles(cacheDir, "*.jpg", SearchOption.TopDirectoryOnly);
        foreach (var file in files)
        {
            var fileName = Path.GetFileNameWithoutExtension(file);
            if (!fileName.StartsWith(currentArticleId + "_"))
                RemovePhotoFromLocal(file);
        }
    }
}