namespace MemoryTrave.Maui.Api;

public static class URL
{
    private const string BaseUrl ="https://localhost:5000";
    
    public static string Registration = $"{BaseUrl}/users/registration";
    public static string Login = $"{BaseUrl}/users/authorization";
}