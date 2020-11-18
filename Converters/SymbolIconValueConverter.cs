using ModernWpf.Controls;
using System;
using System.Globalization;
using System.Windows.Data;

namespace LogViewer3.Converters
{
    public class SymbolIconValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            return new SymbolIcon((Symbol)Enum.Parse(typeof(Symbol), value as string));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
