namespace MemoryTrave.Maui.Models.Profile;

public class MyProfile
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int FriendsCount { get; set; }
    public int ArticlesCount { get; set; }
    public List<GetProfileArticles> Articles { get; set; } = [];
}