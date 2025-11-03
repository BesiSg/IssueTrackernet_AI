using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Utility
{
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class DateToString : IValueConverter
    {
        public object Convert(object value, Type targetType,
        object parameter, System.Globalization.CultureInfo culture)
        {
            return ((DateTime)value).ToString("yyyy-MM-dd");
        }
        public object ConvertBack(object value, Type targetType,
        object parameter, System.Globalization.CultureInfo culture)
        {
            var Date = new DateTime();
            if (DateTime.TryParseExact((string)value, "dd-MM-yyyy", new CultureInfo("en-US"),
                                 DateTimeStyles.None, out Date))
                return Date;
            return DateTime.Now;
        }
    }
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooltoVis : IValueConverter
    {
        public object Convert(object value, Type targetType,
        object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is null) return Visibility.Collapsed;
            return ((bool)value) == true ? Visibility.Visible : Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType,
        object parameter, System.Globalization.CultureInfo culture)
        {
            return ((Visibility)value) == Visibility.Visible;
        }
    }
    [ValueConversion(typeof(string), typeof(bool))]
    public class stringtoBool : IValueConverter
    {
        public object Convert(object value, Type targetType,
        object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is null || (string)value == string.Empty) return false;
            return ((string)value) == "True";
        }
        public object ConvertBack(object value, Type targetType,
        object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? "True" : "False";

        }
    }
}
