using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SupplyRegion.Converters
{
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string? str = value as string;
            bool isInverse = parameter?.ToString() == "inverse";

            if (isInverse)
            {
                return string.IsNullOrEmpty(str) ? Visibility.Visible : Visibility.Collapsed;
            }

            return string.IsNullOrEmpty(str) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
