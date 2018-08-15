using System;
using System.Globalization;
using System.Windows.Data;

namespace YATTS {
    class ModelFlagsToStatusConverter : IMultiValueConverter {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            bool returnColor = "color".Equals(parameter as string, StringComparison.OrdinalIgnoreCase);

            bool connectedToGame = (bool)values[0];
            bool serialOpen = (bool)values[1];
            bool streamingEnabled = (bool)values[2];

            if (streamingEnabled) {
                return returnColor ? (object)System.Windows.Media.Brushes.Green : (object)"ENABLED";
            }
            else {
                if (connectedToGame & serialOpen) {
                    return returnColor ? (object)Consts.GOLDENRODYELLOW : (object)"READY TO BE ENABLED";
                }
                else {
                    return returnColor ? (object)System.Windows.Media.Brushes.Red : (object)"NOT READY";
                }
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
