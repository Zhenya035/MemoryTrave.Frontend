namespace MemoryTrave.Maui.Models.Authorization;

public class AuthResponse
{
    public string JwtToken {get; set;} = string.Empty;
    public Guid UserId { get; set; }
}