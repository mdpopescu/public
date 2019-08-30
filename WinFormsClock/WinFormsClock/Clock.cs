using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WinFormsClock.Helpers;
using WinFormsClock.Models;

namespace WinFormsClock
{
    public partial class Clock : UserControl
    {
        public Clock()
        {
            InitializeComponent();
        }

        //

        private const float HOUR_WIDTH = 6.0f;
        private const float MINUTE_WIDTH = 3.0f;
        private const float SECOND_WIDTH = 1.0f;

        private static readonly Color BACKGROUND = Color.LightBlue;
        private static readonly Color MARKS = Color.Black;
        private static readonly Color HOURS = Color.Red;
        private static readonly Color HANDS = Color.BlueViolet;

        private static void DrawBackground(Graphics g)
        {
            g.Clear(BACKGROUND);
        }

        private static void DrawMinuteMarks(Graphics g, Point origin, int radius)
        {
            using (var pen = new Pen(MARKS))
            {
                foreach (var minute in Enumerable.Range(0, 60))
                {
                    var degree = minute * 6;

                    var lineStart = PolarCoord.Create(degree, radius * 0.90f).CarthesianCoord.Offset(origin);
                    var lineEnd = PolarCoord.Create(degree, radius * 0.95f).CarthesianCoord.Offset(origin);

                    g.DrawLine(pen, lineStart.X, lineStart.Y, lineEnd.X, lineEnd.Y);
                }
            }
        }

        private static void DrawNumbers(Graphics g, Point origin, int radius)
        {
            using (var brush = new SolidBrush(HOURS))
            {
                foreach (var hour in Enumerable.Range(0, 12))
                {
                    var degree = hour * 30;

                    var center = PolarCoord.Create(degree, radius * 0.80f).CarthesianCoord.Offset(origin);
                    var size = Math.Max(24.0f, radius * 0.15f); // don't let the numbers get too small

                    var location = center.Offset(new PointF(-size / 2, -size / 2));

                    // hour 0 should be 12
                    var sHour = hour == 0 ? "12" : hour.ToString();
                    var format = new StringFormat { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center };
                    g.DrawString(sHour, DefaultFont, brush, new RectangleF(location, new SizeF(size, size)), format);
                }
            }
        }

        private static void DrawHandsOrigin(Graphics g, Point origin, int radius)
        {
            using (var brush = new SolidBrush(HANDS))
            {
                var size = radius * 0.05f;
                var rect = new RectangleF(origin.X - size, origin.Y - size, size * 2, size * 2);

                g.FillEllipse(brush, rect);
            }
        }

        private static void DrawHourHand(Graphics g, Point origin, int radius)
        {
            using (var pen = new Pen(HANDS, HOUR_WIDTH))
            {
                var time = DateTime.Now.TimeOfDay;

                var hour = time.Hours % 12;
                var degree = hour * 30;

                var lineStart = PolarCoord.Create(degree, radius * 0.05f).CarthesianCoord.Offset(origin);
                var lineEnd = PolarCoord.Create(degree, radius * 0.50f).CarthesianCoord.Offset(origin);

                g.DrawLine(pen, lineStart.X, lineStart.Y, lineEnd.X, lineEnd.Y);
            }
        }

        private static void DrawMinuteHand(Graphics g, Point origin, int radius)
        {
            using (var pen = new Pen(HANDS, MINUTE_WIDTH))
            {
                var time = DateTime.Now.TimeOfDay;

                var minute = time.Minutes;
                var degree = minute * 6;

                var lineStart = PolarCoord.Create(degree, radius * 0.05f).CarthesianCoord.Offset(origin);
                var lineEnd = PolarCoord.Create(degree, radius * 0.70f).CarthesianCoord.Offset(origin);

                g.DrawLine(pen, lineStart.X, lineStart.Y, lineEnd.X, lineEnd.Y);
            }
        }

        private static void DrawSecondHand(Graphics g, Point origin, int radius)
        {
            using (var pen = new Pen(HANDS, SECOND_WIDTH))
            {
                var time = DateTime.Now.TimeOfDay;

                var second = time.Seconds;
                var degree = second * 6;

                var lineStart = PolarCoord.Create(degree, radius * 0.05f).CarthesianCoord.Offset(origin);
                var lineEnd = PolarCoord.Create(degree, radius * 0.85f).CarthesianCoord.Offset(origin);

                g.DrawLine(pen, lineStart.X, lineStart.Y, lineEnd.X, lineEnd.Y);
            }
        }

        //

        private void Clock_Load(object sender, EventArgs e)
        {
            //
        }

        private void Clock_Paint(object sender, PaintEventArgs e)
        {
            var origin = new Point(e.ClipRectangle.Width / 2, e.ClipRectangle.Height / 2);
            var radius = Math.Min(origin.X, origin.Y);

            DrawBackground(e.Graphics);
            DrawMinuteMarks(e.Graphics, origin, radius);
            DrawNumbers(e.Graphics, origin, radius);

            // draw the hands

            DrawHandsOrigin(e.Graphics, origin, radius);
            DrawHourHand(e.Graphics, origin, radius);
            DrawMinuteHand(e.Graphics, origin, radius);
            DrawSecondHand(e.Graphics, origin, radius);
        }
    }
}