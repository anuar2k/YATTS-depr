using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace YATTS {
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private MemoryRepresentation MemoryRepresentation;

        public MainWindow() {
            InitializeComponent();

            MemoryRepresentation = new MemoryRepresentation();
            DataContext = MemoryRepresentation;

            int sum = 0;
            MemoryRepresentation.StreamedVars.ForEach(var => {
                sum += var.DataSize;
            });

            MemoryRepresentation.EventVariableList.EventVars.ForEach(var => {
                sum += var.DataSize;
            });

            sum += 3; //new data available markers, see EventVariableList.cs
            Title = $"The MemoryRepresentation sum is {sum}";
        }

        private void streamedListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            MemoryRepresentation.Selected = (TelemVar)streamedListView.SelectedItem;
            
            eventListView.SelectionChanged -= eventListView_SelectionChanged;
            eventListView.SelectedItem = null;
            eventListView.SelectionChanged += eventListView_SelectionChanged;
        }

        private void eventListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            MemoryRepresentation.Selected = (TelemVar)eventListView.SelectedItem;

            streamedListView.SelectionChanged -= streamedListView_SelectionChanged;
            streamedListView.SelectedItem = null;
            streamedListView.SelectionChanged += streamedListView_SelectionChanged;
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if (MemoryRepresentation.Selected != null) {
                MemoryRepresentation.Selected.Streamed = !MemoryRepresentation.Selected.Streamed;
            }
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
