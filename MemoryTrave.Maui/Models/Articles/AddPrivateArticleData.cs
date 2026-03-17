using MemoryTrave.Maui.Models.Articles.Access;

namespace MemoryTrave.Maui.Models.Articles;

public class AddPrivateArticleData
{
    public string EncryptedPreviewData { get; set; } = string.Empty;
    public string EncryptedData { get; set; } = string.Empty;
    public List<AddEncryptedKeys> EncryptedKeys { get; set; } = [];
}