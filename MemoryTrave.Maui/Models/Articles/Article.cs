using MemoryTrave.Maui.Models.Enums;

namespace MemoryTrave.Maui.Models.Articles;

public class Article
{
    public Guid Id { get; set; }
    public VisibilityEnum Visibility { get; set; }
    public DateTime LastChange { get; set; }
    public DateTime CreatedAt{ get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public string? EncryptedDescription { get; set; }
    public string? EncryptedKey { get; set; }
}