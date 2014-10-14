using System;
using System.Collections.Generic;
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

    private readonly FontFamily font = new FontFamily("Bookman Old Style");

    private TextBlock CreateTimeLabel()
    {
      var label = new TextBlock
      {
        Name = "TimeLabel",
        Text = "dd MMM yyyy  HH:mm:ss",
        FontFamily = font,
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

    private static void UpdateClock(TextBlock label)
    {
      label.Text = DateTime.Now.ToString("dd MMM yyyy  HH:mm:ss");
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

      var dates = GetDates(first, last).ToList();
      var rowcol = dates.Select(time => GetRowCol(time, first));
      var coords = rowcol.Select(tuple => GetCoords(tuple.Item1, tuple.Item2));
      var labels = coords.Select((tuple, i) =>
      {
        var date = dates[i];
        var color = GetColor(date);
        var effect = GetEffect(date, today);

        return CreateLabel(color, effect, tuple.Item1, tuple.Item2, i + 1);
      });

      foreach (var label in labels)
      {
        Main.Children.Add(label);
      }
    }

    private static IEnumerable<DateTime> GetDates(DateTime first, DateTime last)
    {
      for (var date = first; date <= last; date = date.AddDays(1))
      {
        yield return date;
      }
    }

    private static Tuple<int, int> GetRowCol(DateTime date, DateTime first)
    {
      var col = (int) date.DayOfWeek;
      var row = date.Day / 7 + 1;

      if (date.DayOfWeek < first.DayOfWeek - 1)
        row++;

      return Tuple.Create(row, col);
    }

    private Tuple<double, double> GetCoords(int row, int col)
    {
      var top = Height / 2 + Height / 16 * row;
      var left = Width / 2 + Width / 20 * col;

      return Tuple.Create(top, left);
    }

    private static Color GetColor(DateTime date)
    {
      return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday
        ? Color.FromRgb(255, 0, 0)
        : Color.FromRgb(0, 0, 0);
    }

    private static DropShadowEffect GetEffect(DateTime date, DateTime today)
    {
      return date == today
        ? new DropShadowEffect()
        : null;
    }

    private TextBlock CreateLabel(Color color, Effect effect, double top, double left, int i)
    {
      return new TextBlock
      {
        Name = "CalendarLabel" + i,
        Text = i.ToString(),
        FontFamily = font,
        FontSize = 36,
        Effect = effect,
        VerticalAlignment = VerticalAlignment.Top,
        HorizontalAlignment = HorizontalAlignment.Left,
        TextAlignment = TextAlignment.Right,
        Width = 50,
        Margin = new Thickness(left, top, 0, 0),
        Foreground = new SolidColorBrush(color),
      };
    }

    //

    private void Window_ContentRendered(object sender, EventArgs e)
    {
      UpdateCalendar();
    }
  }
}