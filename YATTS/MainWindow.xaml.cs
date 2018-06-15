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
        private MemoryRepresentation memoryRepresentation;

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

            memoryRepresentation = new MemoryRepresentation();
        }

        private void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
