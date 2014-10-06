using System;
using System.Threading;
using System.Windows;

namespace DesktopClock
{
  /// <summary>
  ///   Interaction logic for MainWindow.xaml
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
  }
}