using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;

namespace YATTS {
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        
        private bool _IsFieldEnabled = false;
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
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            IsFieldEnabled = !IsFieldEnabled;
        }

        private void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
