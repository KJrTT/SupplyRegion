using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace SupplyRegion.View
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModel.MainViewModel viewModel)
            {
                await viewModel.LoadDataCommand.ExecuteAsync(null);
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext is ViewModel.MainViewModel viewModel)
            {
                viewModel.Dispose();
            }
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && DataContext is ViewModel.MainViewModel viewModel)
            {
                viewModel.SearchCommand.Execute(null);
                e.Handled = true;
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModel.MainViewModel viewModel)
            {
                viewModel.LoadDataCommand.Execute(null);
            }
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is ViewModel.MainViewModel viewModel)
            {
                viewModel.EditStatusCommand.Execute(viewModel.SelectedRequest);
            }
        }

        private void ActionsButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.ContextMenu != null)
            {
                // Привязываем DataContext окна (MainViewModel) к контекстному меню,
                // а выбранная строка пойдёт в CommandParameter через PlacementTarget.DataContext
                button.ContextMenu.DataContext = DataContext;
                button.ContextMenu.PlacementTarget = button;
                button.ContextMenu.IsOpen = true;
            }
        }
    }
}
