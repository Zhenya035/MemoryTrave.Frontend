namespace MemoryTrave.Maui.Infrastructure.Api;

public static class URL
{
    private const string BaseUrl ="https://localhost:7196";
    
    public static readonly string Registration = $"{BaseUrl}/users/registration";
    public static readonly string AddKeys = $"{BaseUrl}/users/add-keys";
    
    public static readonly string Login = $"{BaseUrl}/users/authorization";
    public static readonly string GetPrivateKey = $"{BaseUrl}/users/private-key";
}