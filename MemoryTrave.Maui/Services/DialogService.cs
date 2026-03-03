using MemoryTrave.Maui.Services.Interfaces;

namespace MemoryTrave.Maui.Services;

public class DialogService : IDialogService
{
    public async Task ShowMessage(string title, string message, string cancel = "OK") =>
            await Application.Current.MainPage.DisplayAlertAsync(title, message, cancel);
}