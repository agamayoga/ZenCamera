using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Agama
{
    public class VisibilityConverter : IValueConverter
    {
        #region IValueConverter members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visibility = Visibility.Visible;
            if (value != null && value is bool)
            {
                bool p = (bool)value;
                if (parameter is string && ((string)parameter == "Invert" || (string)parameter == "!"))
                {
                    p = !p;
                }
                visibility = (p ? Visibility.Visible : Visibility.Collapsed);
            }
            else if (value is string)
            {
                //Hide if string is null or empty
                string s = value as string;
                bool v = !string.IsNullOrEmpty(s);
                if (parameter is string && !string.IsNullOrEmpty(parameter as string))
                {
                    string p = parameter as string;
                    bool invert = p.StartsWith("!");
                    if (invert)
                    {
                        p = p.Substring(1);
                    }
                    v = (invert ? s != p : s == p);
                }
                visibility = (v ? Visibility.Visible : Visibility.Collapsed);
            }
            else if (value != null && typeof(Enum).IsAssignableFrom(value.GetType())) //If value is enum
            {
                string p = parameter as string;
                visibility = (p == value.ToString() ? Visibility.Visible : Visibility.Collapsed);
            }
            else
            {
                bool p = (value != null);
                if (parameter is string && (string)parameter == "Invert")
                {
                    p = !p;
                }
                else if (parameter is string && (string)parameter != null && value != null)
                {
                    p = (value.ToString() == (string)parameter);
                }
                visibility = (p ? Visibility.Visible : Visibility.Collapsed);
            }
            return visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = false;
            if (value != null && value is Visibility)
            {
                Visibility p = (Visibility)parameter;
                result = (p == Visibility.Visible);
                if (parameter is string && ((string)parameter == "Invert" || (string)parameter == "!"))
                {
                    result = !result;
                }
            }
            return result;
        }
        #endregion
    }
}
