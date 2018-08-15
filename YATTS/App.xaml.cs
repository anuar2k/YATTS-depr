using System;
using System.Windows;
using System.Windows.Forms;

namespace YATTS {
    /// <summary>
    /// Logika interakcji dla klasy App.xaml
    /// </summary>
    public partial class App : System.Windows.Application {
        private Model model = new Model();
        private NotifyIcon icon;

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            icon = new NotifyIcon();
            icon.Icon = System.Drawing.SystemIcons.Exclamation;
            icon.DoubleClick += Icon_DoubleClick;
            icon.Visible = true;

            icon.BalloonTipTitle = "YATTS - test title";
            icon.BalloonTipText = "Double click the icon to open YATTS";
            icon.ShowBalloonTip(10000, icon.BalloonTipTitle, icon.BalloonTipText, ToolTipIcon.None);
        }

        private void Icon_DoubleClick(object sender, EventArgs e) {
            icon.Dispose();
            MainWindow = new MainWindow(model);
            MainWindow.Show();
        }
    }
}
