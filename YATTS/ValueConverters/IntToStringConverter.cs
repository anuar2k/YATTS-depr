using System;
using System.Globalization;
using System.Windows.Data;

namespace YATTS {
    class IntToStringConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is int intValue) {
                return intValue.ToString(CultureInfo.InvariantCulture);
            }
            else {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is string stringValue) {

                if (int.TryParse(stringValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out int intValue)) {
                    return intValue;
                }
                else {
                    return null;
                }
            }
            else {
                return null;
            }
        }
    }
}
