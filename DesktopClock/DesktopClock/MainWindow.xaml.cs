using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

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

      var label = new TextBlock
      {
        Text = "dd MMM yyyy  HH:mm:ss",
        FontFamily = new FontFamily("Bookman Old Style"),
        FontSize = 72,
        VerticalAlignment = VerticalAlignment.Top,
        HorizontalAlignment = HorizontalAlignment.Center,
        TextAlignment = TextAlignment.Center,
        Effect = new DropShadowEffect(),
      };
      Main.Children.Add(label);

      Observable
        .Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1))
        .ObserveOn(this)
        .Subscribe(_ => UpdateClock(label));
    }

    //

    private void UpdateClock(TextBlock label)
    {
      var text = DateTime.Now.ToString("dd MMM yyyy  HH:mm:ss");
      label.Text = text;
    }

    //
  }
}