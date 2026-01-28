using SupplyRegion.Model;
using System.Collections.Generic;
using System.Windows;

namespace SupplyRegion.View
{
    public partial class ChangeStatusWindow : Window
    {
        public string SelectedStatus { get; private set; } = string.Empty;

        public ChangeStatusWindow(string currentStatus)
        {
            InitializeComponent();

            List<string> statuses = PurchaseStatus.GetAll();
            StatusComboBox.ItemsSource = statuses;
            StatusComboBox.SelectedItem = string.IsNullOrWhiteSpace(currentStatus)
                ? PurchaseStatus.New
                : currentStatus;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (StatusComboBox.SelectedItem is string status)
            {
                SelectedStatus = status;
                DialogResult = true;
                Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

