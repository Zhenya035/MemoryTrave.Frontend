using MemoryTrave.Maui.Models.Service;

namespace MemoryTrave.Maui.Services.Interfaces;

public interface IKeyService
{
    public Task<KeyResponse> GenerateKeys(string password);
}