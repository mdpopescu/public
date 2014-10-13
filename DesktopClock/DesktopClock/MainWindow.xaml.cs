using System;
using System.Linq;
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

      var label = CreateTimeLabel();
      Observable
        .Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1))
        .ObserveOn(this)
        .Subscribe(_ => UpdateClock(label));
    }

    //

    private TextBlock CreateTimeLabel()
    {
      var label = new TextBlock
      {
        Name = "TimeLabel",
        Text = "dd MMM yyyy  HH:mm:ss",
        FontFamily = new FontFamily("Bookman Old Style"),
        FontSize = 72,
        VerticalAlignment = VerticalAlignment.Top,
        HorizontalAlignment = HorizontalAlignment.Center,
        TextAlignment = TextAlignment.Center,
        Effect = new DropShadowEffect(),
        Margin = new Thickness(0, 200, 0, 0),
      };
      Main.Children.Add(label);

      return label;
    }

    private void UpdateClock(TextBlock label)
    {
      var text = DateTime.Now.ToString("dd MMM yyyy  HH:mm:ss");
      label.Text = text;
    }

    private void UpdateCalendar()
    {
      RemoveCalendar();
      CreateCalendar();

      // set up the next run at midnight
      Observable
        .Timer(DateTime.Today.AddDays(1).AddSeconds(0.5))
        .ObserveOn(this)
        .Subscribe(_ => UpdateCalendar());
    }

    private void RemoveCalendar()
    {
      var labels = Enumerable
        .Range(1, 31)
        .Select(i => "CalendarLabel" + i)
        .Select(name => Main.Children.OfType<TextBlock>().Where(it => it.Name == name).FirstOrDefault())
        .Where(it => it != null);
      foreach (var label in labels)
      {
        Main.Children.Remove(label);
      }
    }

    private void CreateCalendar()
    {
      var today = DateTime.Today;
      var first = new DateTime(today.Year, today.Month, 1);
      var last = first.AddMonths(1).AddDays(-1);

      var i = 1;
      for (var date = first; date <= last; date = date.AddDays(1), i++)
      {
        var col = (int) date.DayOfWeek;
        var row = date.Day / 7 + 1;

        if (date.DayOfWeek < first.DayOfWeek - 1)
          row++;

        var left = Width / 2 + Width / 20 * col;
        var top = Height / 2 + Height / 16 * row;

        var color = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday ? Color.FromRgb(255, 0, 0) : Color.FromRgb(0, 0, 0);
        var effect = date == today ? new DropShadowEffect() : null;

        var label = new TextBlock
        {
          Name = "CalendarLabel" + i,
          Text = i.ToString(),
          FontFamily = new FontFamily("Bookman Old Style"),
          FontSize = 36,
          Effect = effect,
          VerticalAlignment = VerticalAlignment.Top,
          HorizontalAlignment = HorizontalAlignment.Left,
          TextAlignment = TextAlignment.Right,
          Width = 50,
          Margin = new Thickness(left, top, 0, 0),
          Foreground = new SolidColorBrush(color),
        };
        Main.Children.Add(label);
      }
    }

    //

    private void Window_ContentRendered(object sender, EventArgs e)
    {
      UpdateCalendar();
    }
  }
}