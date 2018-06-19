﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace YATTS {
    class UnitToConvertersConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is Unit unit) {
                if (unit == Unit.NONE || unit == Unit.NULL) {
                    return null;
                } else {
                    return Converters.ConverterDictionary[unit].Keys;
                }
            } else {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
