using System;
using System.Globalization;
using System.Windows.Data;

namespace Agama
{
    public class CheckedConverter : IValueConverter
    {
        #region IValueConverter members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = false;
            if (value != null && value is bool)
            {
                bool p = (bool)value;
                if (parameter is string && ((string)parameter == "Invert" || (string)parameter == "!"))
                {
                    p = !p;
                }
                result = p;
            }
            else if (value is string)
            {
                string s = value as string;
                if (parameter != null)
                {
                    result = (string)parameter == s;
                }
                else
                {
                    result = (s == "true" || s == "1");
                }
            }
            else if (value is int)
            {
                int p = 0;
                int n = (int)value;
                if (parameter != null && int.TryParse((string)parameter, out p))
                {
                    result = (n == p);
                }
                else
                {
                    result = (n == 1);
                }
            }
            else if (value is double)
            {
                double p = 0;
                double n = (double)value;
                if (parameter != null && double.TryParse((string)parameter, out p))
                {
                    result = (n == p);
                }
                else
                {
                    result = (n == 1);
                }
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is bool && targetType == typeof(int))
            {
                int p = 0;
                bool b = (bool)value;
                if (parameter != null && int.TryParse((string)parameter, out p))
                {
                    return b ? p : 0;
                }
                else
                {
                    return b ? 1 : 0;
                }
            }
            else if (value != null && value is bool && targetType == typeof(double))
            {
                double p = 0;
                bool b = (bool)value;
                if (parameter != null && double.TryParse((string)parameter, out p))
                {
                    return b ? p : 0;
                }
                else
                {
                    return b ? 1 : 0;
                }
            }
            return null;
        }
        #endregion
    }
}
