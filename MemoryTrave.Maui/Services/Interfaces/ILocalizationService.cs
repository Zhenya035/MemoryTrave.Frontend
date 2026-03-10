using System.Globalization;

namespace MemoryTrave.Maui.Services.Interfaces;

public interface ILocalizationService
{
    public Task<bool> SetCultureAsync(string cultureCode);
}