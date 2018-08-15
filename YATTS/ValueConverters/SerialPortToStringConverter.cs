using System;
using System.Globalization;
using System.IO.Ports;
using System.Windows.Data;

namespace YATTS {
    class SerialPortToStringConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is SerialPort serialPortValue) {
                if (serialPortValue.IsOpen) {
                    return $"{serialPortValue.PortName} @ {serialPortValue.BaudRate} baud";
                }
            }
            return "CLOSED";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
