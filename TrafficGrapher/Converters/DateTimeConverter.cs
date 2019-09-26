using System;
using System.Globalization;
using System.Windows.Data;

namespace TrafficGrapher.Converters
{
    public class DateTimeConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (double) value;
            var res = new DateTime((long)val);
            return res.ToString("G");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
