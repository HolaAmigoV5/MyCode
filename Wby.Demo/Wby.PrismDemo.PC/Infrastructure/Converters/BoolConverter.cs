using System;
using System.Globalization;
using System.Windows.Data;

namespace Wby.PrismDemo.PC.Infrastructure.Converters
{
    internal class BoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value!=null && int.TryParse(value.ToString(),out int result))
            {
                if (result == 0)
                    return false;
                return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && bool.TryParse(value.ToString(), out bool result))
            {
                return result ? 1 : (object)0;
            }
            return 0;
        }
    }
}
