using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace YATTS {
    class BoolToFontWeightConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is bool boolValue) {
                return boolValue ? FontWeights.Bold : FontWeights.Normal;
            } else {
                return FontWeights.Normal;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
