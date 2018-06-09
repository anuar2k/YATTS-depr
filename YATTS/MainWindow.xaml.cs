using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace YATTS {
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();

            ViewModel model = new ViewModel();
            DataContext = model;
            
            Task.Factory.StartNew(() => {
                while (true) {
                    model.IsFieldEnabled = !model.IsFieldEnabled;
                    Thread.Sleep(1000);
                }
            });
        }
    }
}
