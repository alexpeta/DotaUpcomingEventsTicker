using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace DotaUpcomingEventsTicker.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool castedValueVisibiltyBool = false;

            if (value is bool)
            {
                castedValueVisibiltyBool = (bool)value;
            }

            if (parameter != null)
            {
                if (bool.Parse((string)parameter))
                {
                    castedValueVisibiltyBool = !castedValueVisibiltyBool;
                }
            }

            if (castedValueVisibiltyBool)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool visibility = (value is Visibility) && (((Visibility)value) == Visibility.Visible);

            if (parameter != null)
            {
                if (bool.Parse((string)parameter))
                {
                    visibility = !visibility;
                }
            }

            if (visibility)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
