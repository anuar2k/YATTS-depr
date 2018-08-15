using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace YATTS {
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private Model Model;

        public MainWindow(Model Model) {
            this.Model = Model;

            InitializeComponent();
            DataContext = Model;
        }

        private void streamedListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            Model.Selected = (TelemVar)streamedListView.SelectedItem;

            eventListView.SelectionChanged -= eventListView_SelectionChanged;
            eventListView.SelectedItem = null;
            eventListView.SelectionChanged += eventListView_SelectionChanged;
            UpdateValue();
        }

        private void eventListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            Model.Selected = (TelemVar)eventListView.SelectedItem;

            streamedListView.SelectionChanged -= streamedListView_SelectionChanged;
            streamedListView.SelectedItem = null;
            streamedListView.SelectionChanged += streamedListView_SelectionChanged;
            UpdateValue();
        }

        private void UpdateValue() {
            if (Model.Selected != null) {
                if (Model.ConnectedToGame) {
                    string text = Model.Selected.GetStringValue(Model.MMVA);
                    valueTextBox.Text = text;
                }
            }
            else {
                valueTextBox.Text = String.Empty;
            }
        }

        private Regex baudrateRegex = new Regex("[^0-9]+");
        private void BaudrateTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            e.Handled = baudrateRegex.IsMatch(e.Text);
        }

        private void BaudrateTextBox_Pasting(object sender, DataObjectPastingEventArgs e) {
            if (e.DataObject.GetDataPresent(typeof(String))) {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (baudrateRegex.IsMatch(text)) {
                    e.CancelCommand();
                }
            }
            else {
                e.CancelCommand();
            }
        }

        private void GameButton_Click(object sender, RoutedEventArgs e) {
            if (Model.ConnectedToGame) {
                Model.DisconnectFromGame();
                MessageBox.Show("Game disconnected.", "Disconnect", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else {
                if (Model.ConnectToGame()) {
                    MessageBox.Show("Game successfully connected!", "Connect", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else {
                    MessageBox.Show("Connection attempt failed. You must run your game first.", "Connect", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SerialButton_Click(object sender, RoutedEventArgs e) {
            if (Model.SerialOpen) {
                Model.CloseSerial();
            }
            else {
                switch (Model.OpenSerial()) {
                    case SerialOpenResults.OK:
                        MessageBox.Show("Serial port successfully opened.", "Open serial port", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    case SerialOpenResults.FAILED_PORTNAME:
                        MessageBox.Show("The serial port name you entered is wrong.", "Open serial port", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    case SerialOpenResults.FAILED_NOTFOUND:
                        MessageBox.Show($"There is no serial port named {Model.SerialPortName} available.", "Open serial port", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    case SerialOpenResults.FAILED_ALREADYUSED:
                        MessageBox.Show($"Port {Model.SerialPortName} is already used by another app. You need to close it first (release the port).", "Open serial port", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    case SerialOpenResults.FAILED_BAUDRATEOOR:
                        MessageBox.Show($"Port {Model.SerialPortName} doesn't support {Model.SerialPortBaudrate} baud (it might exceed the maximum possible value).", "Open serial port", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    case SerialOpenResults.FAILED_BAUDRATENULL:
                        MessageBox.Show("You must enter baudrate first.", "Open serial port", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    case SerialOpenResults.FAILED_UNKNOWN:
                        MessageBox.Show($"Unknown exception, GetLastError: {Marshal.GetLastWin32Error()} - report it to me!", "Open serial port", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                }
            }
        }
    }
}
