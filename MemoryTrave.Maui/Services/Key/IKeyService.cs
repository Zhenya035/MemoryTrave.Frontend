using MemoryTrave.Maui.Models.Service;

namespace MemoryTrave.Maui.Services.Key;

public interface IKeyService
{
    public Task<KeyResponse> GenerateKeys(string password);
}