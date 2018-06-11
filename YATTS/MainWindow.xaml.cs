using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace YATTS {
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        
        private bool _IsFieldEnabled = true;
        public bool IsFieldEnabled {
            get {
                return _IsFieldEnabled;
            }
            set {
                if (value != IsFieldEnabled) {
                    _IsFieldEnabled = value;
                    OnPropertyChanged(nameof(IsFieldEnabled));
                }
            }
        }

        public MainWindow() {
            InitializeComponent();
            DataContext = this;

            TelemVar ttv = new FloatTelemVar("ID", "Name", "Desc", 1000, Consts.TRUCK_WHEEL_COUNT);
            Debug.WriteLine(ttv.ArrayLength);
            Debug.WriteLine(ttv.DataSize);
            Debug.WriteLine(ttv.Description);
            Debug.WriteLine(ttv.ElementSize);
            Debug.WriteLine(ttv.ID);
            Debug.WriteLine(ttv.MaxArrayLength);
            Debug.WriteLine(ttv.Name);
            Debug.WriteLine(ttv.Offset);
            Debug.WriteLine(ttv.TypeName);

            if (ttv is FloatTelemVar ttvf) {
                ttvf.ArrayLength = 4;
                Debug.WriteLine(ttv.ArrayLength);
                Debug.WriteLine(ttv.DataSize);
                Debug.WriteLine(ttv.ElementSize);
                Debug.WriteLine(ttv.MaxArrayLength);
                ttvf.ConvertToInt = true;
                Debug.WriteLine(ttv.TypeName);
            }
        }

        private void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
