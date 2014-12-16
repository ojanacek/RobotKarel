using System;
using System.Windows.Data;

namespace RobotKarel.Klient
{
    /// <summary>
    /// Převádí X-ové souřadnice na správnou vzdálenost v pixelech.
    /// </summary>
    [ValueConversion(typeof(int), typeof(int))]
    class PositionConverterX : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((int)value + 21) * 25;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Převádí Y-ové souřadnice na správnou vzdálenost v pixelech.
    /// </summary>
    [ValueConversion(typeof(int), typeof(int))]
    class PositionConverterY : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (-(int)value + 21) * 25;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
