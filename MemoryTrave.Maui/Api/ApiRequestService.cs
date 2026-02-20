using System.Net.Http.Json;

namespace MemoryTrave.Maui.Api;

public class ApiRequestService(HttpClient client)
{
    public async Task<T?> GetRequest<T>(string url)
    {
        using var response = await client.GetAsync(url);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<T>();
            
        return default;
    }
    
    public async Task<TResponse?> PostRequest<TRequest, TResponse>(string url, TRequest body)
    {
        using var response = await client.PostAsJsonAsync(url, body);
        
        if(response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<TResponse>();
        
        return default;
    }

    public async Task<TResponse?> PutRequest<TRequest, TResponse>(string url, TRequest body)
    {
        using var response = await client.PutAsJsonAsync(url, body);

        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<TResponse>();
        
        return default;
    }

    public async Task<bool> DeleteRequest(string url)
    {
        using var response = await client.DeleteAsync(url);
        return response.IsSuccessStatusCode;
    }
}