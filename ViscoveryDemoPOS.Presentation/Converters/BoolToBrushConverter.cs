using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ViscoveryDemoPOS.Presentation.Converters
{
    /// <summary>
    /// Converts a boolean value to one of two <see cref="Brush"/> instances.
    /// Used for styling elements based on success/failure conditions.
    /// </summary>
    public class BoolToBrushConverter : IValueConverter
    {
        /// <summary>Brush returned when the input is <c>true</c>.</summary>
        public Brush TrueBrush { get; set; } = Brushes.Green;

        /// <summary>Brush returned when the input is <c>false</c>.</summary>
        public Brush FalseBrush { get; set; } = Brushes.Red;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return b ? TrueBrush : FalseBrush;
            return FalseBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
