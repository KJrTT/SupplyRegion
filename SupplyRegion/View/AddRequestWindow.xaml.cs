using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SupplyRegion.View
{
    public partial class AddRequestWindow : Window
    {
        public AddRequestWindow()
        {
            InitializeComponent();

            // Устанавливаем DataContext после инициализации
            DataContext = new ViewModel.AddRequestViewModel(this);

            Loaded += Window_Loaded;
            KeyDown += Window_KeyDown;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Устанавливаем фокус на первое поле
            var initiatorTextBox = Template.FindName("InitiatorTextBox", this) as TextBox;
            initiatorTextBox?.Focus();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void DecimalValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.,]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                CancelButton_Click(sender, e);
            }
        }
    }
}
