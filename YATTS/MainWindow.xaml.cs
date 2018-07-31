﻿using System;
using System.Collections.Generic;
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
            UpdateValue();
        }

        private void eventListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            MemoryRepresentation.Selected = (TelemVar)eventListView.SelectedItem;

            streamedListView.SelectionChanged -= streamedListView_SelectionChanged;
            streamedListView.SelectedItem = null;
            streamedListView.SelectionChanged += streamedListView_SelectionChanged;
            UpdateValue();
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if (MemoryRepresentation.Selected != null) {
                MemoryRepresentation.Selected.Streamed = !MemoryRepresentation.Selected.Streamed;
            }
        }

        private void ConvertButton_Click(object sender, RoutedEventArgs e) {

        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            MemoryRepresentation.Hook();
            Task.Factory.StartNew(() => {
                while (true) {
                    Dispatcher.Invoke(new Action(UpdateValue));
                    Thread.Sleep(100);
                }
            });
        }

        private void UpdateValue() {
            if (MemoryRepresentation.Selected != null) {
                if (MemoryRepresentation.MMVA?.CanRead ?? false) {
                    string text = MemoryRepresentation.Selected.GetStringValue(MemoryRepresentation.MMVA);
                    valueTextBox.Text = text;
                }
            }
            else {
                valueTextBox.Text = String.Empty;
            }
        }
    }

    public static class Helper {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action) {
            foreach (var cur in enumerable) {
                action(cur);
            }
        }
    }
}
