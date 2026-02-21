using System.Text.Json;

namespace MemoryTrave.Maui.Api;

public static class ApiErrorParser
{
    public static string ToUserFriendlyMessage(this string? errorJson)
    {
        if (string.IsNullOrEmpty(errorJson))
            return "Произошла неизвестная ошибка";
        
        try
        {
            using var json = JsonDocument.Parse(errorJson);
            var root = json.RootElement;
            var messages = new HashSet<string>();
            
            if (root.ValueKind == JsonValueKind.Array)
            {
                foreach (var error in root.EnumerateArray())
                {
                    ExtractErrorMessage(error, messages);
                }
            }
            else if (root.ValueKind == JsonValueKind.Object)
            {
                ExtractErrorMessage(root, messages);
            }
            
            return messages.Count > 0 
                ? string.Join("\n", messages) 
                : "Произошла неизвестная ошибка";
        }
        catch
        {
            return errorJson;
        }
    }
    
    private static void ExtractErrorMessage(JsonElement element, HashSet<string> messages)
    {
        string[] possibleFields = { 
            "Detailed", "Message", "message", "error", 
            "errorMessage", "ErrorMessage", "detail" 
        };
        
        foreach (var field in possibleFields)
        {
            if (element.TryGetProperty(field, out var prop))
            {
                var message = prop.GetString();
                if (!string.IsNullOrEmpty(message))
                {
                    messages.Add(message);
                }
            }
        }
        
        if (element.TryGetProperty("errorMessage", out var errorMsg))
        {
            var message = errorMsg.GetString();
            if (!string.IsNullOrEmpty(message))
                messages.Add(message);
        }
    }
}