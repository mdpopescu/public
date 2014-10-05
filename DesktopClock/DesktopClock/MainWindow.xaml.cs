using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace DesktopClock
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            timer = new Timer(_ => UpdateClock(), null, 0, 1000);
        }

        //

        private readonly Timer timer;

        private void UpdateClock()
        {
            var text = DateTime.Now.ToString("dd MMM yyyy  HH:mm:ss");
            this.UIChange(_ => LblClock.Text = text, text);
        }

        //

        private void Window_Initialized(object sender, System.EventArgs e)
        {
            //LblClock.Margin = new Thickness(0, 0, 0, 0);
        }
    }
}