using MemoryTrave.Maui.Resources.Localization;
using MemoryTrave.Maui.Services.Interfaces;

namespace MemoryTrave.Maui.Services;

public class DialogService : IDialogService
{
    public async Task ShowMessage(string title, string message, string? cancel = null) =>
            await Application.Current.MainPage.DisplayAlertAsync(title, message, cancel ?? Localization.Ok);
}