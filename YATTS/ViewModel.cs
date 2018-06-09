using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace YATTS {
    public class ViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _IsFieldEnabled = false;
        public bool IsFieldEnabled {
            get {
                return _IsFieldEnabled;
            }
            set {
                if (value != _IsFieldEnabled) {
                    _IsFieldEnabled = value;
                    OnPropertyChanged(nameof(IsFieldEnabled));
                }
            }
        }

        private void OnPropertyChanged(String propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
