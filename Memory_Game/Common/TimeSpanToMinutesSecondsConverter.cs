using System;
using System.Globalization;
using System.Windows.Data;

namespace Memory_Game.View
{
    public class TimeSpanToMinutesSecondsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan timeSpan)
            {
                return $"{(int)timeSpan.TotalMinutes:D2}:{timeSpan.Seconds:D2}";
            }
            return "00:00";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}