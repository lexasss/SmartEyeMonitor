using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace SmartEyeMonitor.Converters;

[ValueConversion(typeof(bool), typeof(Visibility))]
public class VisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        (bool)value ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => 
        (Visibility)value == Visibility.Visible;
}
