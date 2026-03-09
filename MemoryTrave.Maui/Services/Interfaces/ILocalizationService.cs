using System.Globalization;

namespace MemoryTrave.Maui.Services.Interfaces;

public interface ILocalizationService
{
    public Task SetCultureAsync(string cultureCode);
}