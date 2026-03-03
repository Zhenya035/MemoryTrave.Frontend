using System.Globalization;

namespace MemoryTrave.Maui.Services.Interfaces;

public interface ILocalizationService
{
    CultureInfo CurrentCulture { get; }
    Task SetCultureAsync(string cultureCode);
    event Action CultureChanged;
}