namespace MemoryTrave.Maui.Infrastructure.Api;

public static class URL
{
    private const string BaseUrl = "https://localhost:7196";
    
    //Article
    private const string ArticleUrl = $"{BaseUrl}/articles";
    public static string GetArticleById(Guid articleId) => $"{ArticleUrl}/{articleId.ToString()}";
    public static string AddPrivateArticle() => $"{ArticleUrl}/private";
    public static string AddPublicArticle() => $"{ArticleUrl}/public";
    public static string UpdateArticle(Guid articleId) => $"{ArticleUrl}/{articleId.ToString()}";
    public static string DeleteArticle(Guid articleId) => $"{ArticleUrl}/{articleId.ToString()}";
    
    //Location
    private const string LocationUrl = $"{BaseUrl}/locations";
    public static string GetLocations() => $"{LocationUrl}";
    public static string GetLocationById(Guid locationId) => $"{LocationUrl}/{locationId.ToString()}";
    public static string AddLocation() => $"{LocationUrl}";
    public static string UpdateLocation(Guid locationId) => $"{LocationUrl}/{locationId.ToString()}";
    public static string DeleteLocation(Guid locationId) => $"{LocationUrl}/{locationId.ToString()}";
    
    //Friendship
    private const string FriendshipUrl = $"{BaseUrl}/friends";
    public static string GetFriends() => $"{FriendshipUrl}";
    public static string DeleteFriendship(Guid friendshipId) => $"{FriendshipUrl}/{friendshipId.ToString()}";
    
    //Friend Request
    private const string FriendRequestUrl = $"{BaseUrl}/friends/requests";
    public static string GetRequests() => $"{FriendRequestUrl}";
    public static string AddRequest() => $"{FriendRequestUrl}";
    public static string ConfirmRequest(Guid requestId) => $"{FriendRequestUrl}/{requestId.ToString()}/confirm";
    public static string CancelRequest(Guid requestId) => $"{FriendRequestUrl}/{requestId.ToString()}/cancel";
    
    //Auth
    private const string AuthUrl = $"{BaseUrl}/users/auth";
    public static string GetEncryptedPrivateKey() => $"{AuthUrl}/keys/private";
    public static string Registration() => $"{AuthUrl}/registration";
    public static string Authorization() => $"{AuthUrl}/authorization";
    public static string AddKeys() => $"{AuthUrl}/add/keys";
    
    //Profile
    private const string ProfileUrl = $"{BaseUrl}/users/profile";
    public static string GetProfile() => $"{ProfileUrl}/my";
    public static string GetBlockedUsers() => $"{ProfileUrl}/blocks";
    
    //User
    private const string UserUrl = $"{BaseUrl}/users";
    public static string GetUsersWithoutMe() => $"{UserUrl}/available";
    public static string BlockUserPersonally() => $"{UserUrl}/block"; 
    public static string UnblockUserPersonally() => $"{UserUrl}/unblock";
    public static string DeleteUser() => $"{UserUrl}";
}