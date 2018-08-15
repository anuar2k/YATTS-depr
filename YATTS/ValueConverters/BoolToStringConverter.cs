using System;
using System.Globalization;
using System.Windows.Data;

namespace YATTS {
    class BoolToStringConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is bool boolValue) {
                if (parameter is string stringParameter) {
                    switch (stringParameter.ToLower()) {
                        case "caps-onoff":
                            return boolValue ? "ON" : "OFF";
                        case "inverted-onoff-btn":
                            return boolValue ? "Turn off" : "Turn on";
                        case "inverted-open-btn":
                            return boolValue ? "Close" : "Open";
                        case "inverted-connect-btn":
                            return boolValue ? "Disconnect" : "Connect";
                        case "caps-connected":
                            return boolValue ? "CONNECTED" : "NOT CONNECTED";
                        default:
                            return Binding.DoNothing;
                    }
                }
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
