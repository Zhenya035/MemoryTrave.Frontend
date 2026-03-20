namespace MemoryTrave.Maui.Infrastructure.Api;

public static class URL
{
    private const string BaseUrl = "https://memorytrave.somee.com";
    
    //Article
    private const string ArticleUrl = $"{BaseUrl}/articles";
    public static string GetArticleById(string articleId) => $"{ArticleUrl}/{articleId.ToString()}";
    public static string AddPrivateArticle(string locationId) => $"{ArticleUrl}/private/{locationId}/create";
    public static string AddPublicArticle() => $"{ArticleUrl}/public";
    public static string AddDataToPrivateArticle(string articleId) => $"{ArticleUrl}/private/{articleId}/data";
    public static string UpdateArticle(string articleId) => $"{ArticleUrl}/{articleId.ToString()}";
    public static string DeleteArticle(string articleId) => $"{ArticleUrl}/{articleId.ToString()}";
    
    //Location
    private const string LocationUrl = $"{BaseUrl}/locations";
    public static string GetLocations() => $"{LocationUrl}";
    public static string GetLocationById(string locationId) => $"{LocationUrl}/{locationId.ToString()}";
    public static string AddLocation() => $"{LocationUrl}";
    public static string UpdateLocation(string locationId) => $"{LocationUrl}/{locationId.ToString()}";
    public static string DeleteLocation(string locationId) => $"{LocationUrl}/{locationId.ToString()}";
    
    //Friendship
    private const string FriendshipUrl = $"{BaseUrl}/friends";
    public static string GetFriends() => $"{FriendshipUrl}";
    public static string GetFriendsPublicKeys() => $"{FriendshipUrl}/keys";
    public static string DeleteFriendship(string friendshipId) => $"{FriendshipUrl}/{friendshipId.ToString()}";
    
    //Friend Request
    private const string FriendRequestUrl = $"{BaseUrl}/friends/requests";
    public static string GetRequests(int direction) => $"{FriendRequestUrl}?direction={direction}";
    public static string AddRequest() => $"{FriendRequestUrl}";
    public static string ConfirmRequest(string requestId) => $"{FriendRequestUrl}/{requestId.ToString()}/confirm";
    public static string CancelRequest(string requestId) => $"{FriendRequestUrl}/{requestId.ToString()}/cancel";
    
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
    public static string GetPublicKey() => $"{UserUrl}/keys/public";
    public static string BlockUserPersonally() => $"{UserUrl}/block"; 
    public static string UnblockUserPersonally() => $"{UserUrl}/unblock";
    public static string DeleteUser() => $"{UserUrl}";
    
    //Photo
    private const string PhotoUrl = $"{BaseUrl}/photos";
    public static string GetPhotosFromArticle() => $"{PhotoUrl}/download";
    public static string GetPhotoByKey() => $"{PhotoUrl}/file";
    public static string UploadPhoto(string articleId) => $"{PhotoUrl}/{articleId}/upload";
    public static string DeletePhotosByArticle(string articleId) => $"{PhotoUrl}/{articleId}/all";
    public static string DeletePhotosByKeys(string articleId) => $"{PhotoUrl}/{articleId}/all";
}