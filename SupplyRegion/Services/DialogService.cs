using System.Windows;

namespace SupplyRegion.Services; 

public class DialogService
{
    public bool? ShowDialog(Window window)
    {
        window.Owner = Application.Current.MainWindow;
        return window.ShowDialog();
    }

    public void ShowMessage(string message, string title = "Сообщение")
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public bool ShowConfirmation(string message, string title = "Подтверждение")
    {
        return MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question)
               == MessageBoxResult.Yes;
    }

    public string ShowSaveFileDialog(string filter, string defaultFileName)
    {
        var dialog = new Microsoft.Win32.SaveFileDialog
        {
            Filter = filter,
            FileName = defaultFileName,
            DefaultExt = ".csv"
        };

        return dialog.ShowDialog() == true ? dialog.FileName : string.Empty;
    }
}
