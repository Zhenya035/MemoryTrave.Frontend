namespace MemoryTrave.Maui.Services.Dialog;

public class DialogService : IDialogService
{
    public async Task ShowMessage(string title, string message, string? cancel = null) =>
        await MainThread.InvokeOnMainThreadAsync(async () =>
            await Application.Current.MainPage.DisplayAlertAsync(title, message,
                cancel ?? Resources.Localization.Localization.Ok));
}