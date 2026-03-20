namespace MemoryTrave.Maui.Services.Localization;

public interface ILocalizationService
{
    public Task<bool> SetCultureAsync(string cultureCode);
}