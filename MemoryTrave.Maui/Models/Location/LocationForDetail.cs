using MemoryTrave.Maui.Models.Articles;

namespace MemoryTrave.Maui.Models.Location;

public class LocationForDetail
{
    public string Name { get; set; } = string.Empty;
    public List<Article> Articles { get; set; } = [];
}