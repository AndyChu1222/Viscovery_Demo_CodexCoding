using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ViscoveryDemoPOS.Presentation.Converters
{
    /// <summary>
    /// Converts a boolean into the opposite <see cref="Visibility"/> value. A
    /// value of <c>true</c> becomes <see cref="Visibility.Collapsed"/> and
    /// <c>false</c> becomes <see cref="Visibility.Visible"/>.
    /// </summary>
    public class InverseBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return b ? Visibility.Collapsed : Visibility.Visible;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
