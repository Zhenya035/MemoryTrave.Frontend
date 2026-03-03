using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace MemoryTrave.Maui.Infrastructure.Api;

public class ApiRequestService(HttpClient client)
{
    public void SetJwtToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("JWT token cannot be null or empty.");
        
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }
    
    public async Task<ApiResult<T>> GetRequest<T>(string url)
    {
        try
        {
            using var response = await client.GetAsync(url);
        
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<T>();
            
                return ApiResult<T>.Success(result, (int)response.StatusCode);
            }
        
            string errorMessage = await response.Content.ReadAsStringAsync();
        
            return ApiResult<T>.Failure(errorMessage, (int)response.StatusCode);
        }
        catch (Exception e)
        {
            var errorMessage = e.Message;
            return ApiResult<T>.Failure(errorMessage);
        }
    }
    
    public async Task<ApiResult<TResponse>> PostRequest<TRequest, TResponse>(string url, TRequest body)
    {
        try
        {
            using var response = await client.PostAsJsonAsync(url, body);
        
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<TResponse>();
            
                return ApiResult<TResponse>.Success(result, (int)response.StatusCode);
            }
        
            string errorMessage = await response.Content.ReadAsStringAsync();
        
            return ApiResult<TResponse>.Failure(errorMessage, (int)response.StatusCode);
        }
        catch (Exception e)
        {
            var errorMessage = e.Message;
            return ApiResult<TResponse>.Failure(errorMessage);
        }
    }

    public async Task<ApiResult<bool>> PutRequest<TRequest>(string url, TRequest body)
    {
        try
        {
            using var response = await client.PutAsJsonAsync(url, body);
            
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    return ApiResult<bool>.Success(true, (int)response.StatusCode);
                
                var result = await response.Content.ReadFromJsonAsync<bool>();
            
                return ApiResult<bool>.Success(result, (int)response.StatusCode);
            }
        
            string errorMessage = await response.Content.ReadAsStringAsync();
        
            return ApiResult<bool>.Failure(errorMessage, (int)response.StatusCode);
        }
        catch (Exception e)
        {
            var errorMessage = e.Message;
            return ApiResult<bool>.Failure(errorMessage);
        }
    }

    public async Task<ApiResult<bool>> DeleteRequest(string url)
    {
        try
        {
            using var response = await client.DeleteAsync(url);
        
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<bool>();
            
                return ApiResult<bool>.Success(result, (int)response.StatusCode);
            }
        
            string errorMessage = await response.Content.ReadAsStringAsync();
        
            return ApiResult<bool>.Failure(errorMessage, (int)response.StatusCode);
        }
        catch (Exception e)
        {
            var errorMessage = e.Message;
            return ApiResult<bool>.Failure(errorMessage);
        }
    }
}