using System;
using System.Globalization;
using System.Windows.Data;

namespace YATTS {
    class UnitToEnabledConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is Unit unit) {
                if (unit == Unit.NONE || unit == Unit.NULL) {
                    return false;
                } else {
                    return true;
                }
            } else {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
