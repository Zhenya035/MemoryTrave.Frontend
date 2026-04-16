namespace MemoryTrave.Maui.Models.Articles.Access;

public class AddEncryptedKeysForAddFriend
{
    public string EncryptedKey {get; set;} = string.Empty;
    public Guid ArticleId { get; set; }
}