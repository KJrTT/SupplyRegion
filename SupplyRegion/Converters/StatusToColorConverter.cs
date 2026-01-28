using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using SupplyRegion.Model;

namespace SupplyRegion.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return status switch
                {
                    PurchaseStatus.New => new SolidColorBrush(Color.FromArgb(255, 59, 130, 246)), 
                    PurchaseStatus.Approved => new SolidColorBrush(Color.FromArgb(255, 16, 185, 129)),
                    PurchaseStatus.Ordered => new SolidColorBrush(Color.FromArgb(255, 245, 158, 11)), 
                    PurchaseStatus.Received => new SolidColorBrush(Color.FromArgb(255, 148, 163, 184)), 
                    PurchaseStatus.Cancelled => new SolidColorBrush(Color.FromArgb(255, 239, 68, 68)), 
                    _ => new SolidColorBrush(Colors.White)
                };
            }

            return new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
