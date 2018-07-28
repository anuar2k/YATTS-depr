using System;
using System.Globalization;
using System.Windows.Data;

namespace YATTS {
    class TelemVarToQuantityConverter : IMultiValueConverter {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            int ArrayLength = (int)values[0];
            int MaxArrayLength = (int)values[1];
            return ArrayLength == MaxArrayLength ? MaxArrayLength.ToString() : $"{ArrayLength}/{MaxArrayLength}";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
