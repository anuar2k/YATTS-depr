using System;
using System.Globalization;
using System.Windows.Data;

namespace YATTS {
    class CastModeToRadioConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is CastMode castMode) {
                if (parameter is string strParameter) {
                    if (castMode.ToString().Equals(strParameter, StringComparison.InvariantCultureIgnoreCase)) {
                        return true;
                    } else {
                        return false;
                    }
                } else {
                    return false;
                }
            } else {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is bool boolValue) {
                if (boolValue) {
                    if (parameter is string strParameter) {
                        return Enum.Parse(targetType, strParameter, true);
                    } else {
                        return Binding.DoNothing;
                    }
                } else {
                    return Binding.DoNothing;
                }
            } else {
                return Binding.DoNothing;
            }
        }
    }
}
