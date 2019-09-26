using System;
using System.Globalization;
using System.Windows.Data;

namespace TrafficGrapher.Converters
{
    public class BandwidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (double) value;
            if (val >= 1000)
            {
                return $"{Math.Round(val / 1000, 2)}G";
            }

            return val >= 1 ? $"{Math.Round(val, 2)}M" : $"{Math.Round(val * 1000, 2)}K";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
