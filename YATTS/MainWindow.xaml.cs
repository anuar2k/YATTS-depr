using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static YATTS.Helper;

namespace YATTS {
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        public MemoryRepresentation MemoryRepresentation { get; private set; }
        private int index = 0;

        public MainWindow() {
            InitializeComponent();

            DataContext = this;

            MemoryRepresentation = new MemoryRepresentation();

            int sum = 0;
            MemoryRepresentation.StreamedVars.ForEach(var => {
                sum += var.DataSize;
            });

            MemoryRepresentation.EventVars.TruckData.ForEach(var => {
                sum += var.DataSize;
            });
            MemoryRepresentation.EventVars.TrailerData.ForEach(var => {
                sum += var.DataSize;
            });
            MemoryRepresentation.EventVars.JobData.ForEach(var => {
                sum += var.DataSize;
            });

            sum += 3; //new data available markers, see EventVariableList.cs
            Debug.WriteLine(sum);

            streamedListView.ItemsSource = MemoryRepresentation.StreamedVars;

            Task.Factory.StartNew(() => {
                bool go = true;
                while(go) {
                    Thread.Sleep(1000);
                    MemoryRepresentation.Selected = MemoryRepresentation.StreamedVars[index++];
                    if (index > 4) {
                        go = false;
                    }
                }
            });
        }

        private void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public static class Helper {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action) {
            foreach(var cur in enumerable) {
                action(cur);
            }
        }
    }
}
