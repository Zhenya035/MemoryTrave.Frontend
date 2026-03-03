namespace MemoryTrave.Maui.Services.Interfaces;

public interface IDialogService
{
    Task ShowMessage(string title, string message, string cancel = "OK");
}