namespace MemoryTrave.Maui.Models.Profile;

public class GetProfileArticles
{
    public Guid Id { get; set; }
    public string? LocationName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime LastChange { get; set; }
    public bool IsPrivate { get; set; }
    
    public string? EncryptedPreviewData  { get; set; }
    public string? EncryptedKey { get; set; }
    
    public string? Description { get; set; }
}