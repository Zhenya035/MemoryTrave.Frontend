namespace MemoryTrave.Maui.Services.Dialog;

public interface IDialogService
{
    Task ShowMessage(string title, string message, string cancel = "OK");
}