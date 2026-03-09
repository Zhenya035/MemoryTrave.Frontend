namespace MemoryTrave.Maui.Models.Profile;

public class ProfileArticles
{
    public Guid Id { get; set; }
    public string? LocationName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime LastChange { get; set; }
    public string IsPrivate { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}