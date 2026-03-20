namespace MemoryTrave.Maui.Models.Articles.Access;

public class AddEncryptedKeys
{
    public string EncryptedKey {get; set;} = string.Empty;
    public Guid UserId { get; set; }
}