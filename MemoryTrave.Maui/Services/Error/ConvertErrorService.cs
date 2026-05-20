namespace MemoryTrave.Maui.Services.Error;

public class ConvertErrorService : IConvertErrorService
{
    public string ConvertError(int? status)
    {
        var message = status switch
        {
            400 => Resources.Localization.Localization._400Error,
            401 => Resources.Localization.Localization._401Error,
            403 => Resources.Localization.Localization._403Error,
            404 => Resources.Localization.Localization._404Error,
            409 => Resources.Localization.Localization._409Error, 
            _ => Resources.Localization.Localization.DefaultError
        };

        return message;
    }
}