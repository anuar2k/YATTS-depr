using System;
using System.Globalization;
using System.Windows.Data;

namespace YATTS {
    class BoolToColorConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is bool boolValue) {
                return boolValue ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Red;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
