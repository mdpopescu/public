using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WinFormsClock.Helpers;
using WinFormsClock.Models;

namespace WinFormsClock
{
    public partial class ClockBackground : UserControl
    {
        public ClockBackground()
        {
            InitializeComponent();
        }

        //

        private static readonly Color BACKGROUND = Color.LightBlue;
        private static readonly Color MARKS = Color.Black;
        private static readonly Color HOURS = Color.Red;

        //

        private void ClockBackground_Load(object sender, EventArgs e)
        {
            //
        }

        private void ClockBackground_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(BACKGROUND);

            var origin = new Point(e.ClipRectangle.Width / 2, e.ClipRectangle.Height / 2);
            var radius = Math.Min(origin.X, origin.Y);

            // draw the minute marks
            using (var pen = new Pen(MARKS))
            {
                foreach (var minute in Enumerable.Range(0, 60))
                {
                    var degree = minute * 6;

                    var lineStart = PolarCoord.Create(degree, radius * 0.90f).CarthesianCoord.Offset(origin);
                    var lineEnd = PolarCoord.Create(degree, radius * 0.95f).CarthesianCoord.Offset(origin);

                    e.Graphics.DrawLine(pen, lineStart.X, lineStart.Y, lineEnd.X, lineEnd.Y);
                }
            }

            // draw the numbers
            using (var brush = new SolidBrush(HOURS))
            {
                foreach (var hour in Enumerable.Range(1, 12))
                {
                    var degree = (hour - 1) * 30;

                    // 12 should be at the top
                    degree = (degree + 300) % 360;

                    var center = PolarCoord.Create(degree, radius * 0.80f).CarthesianCoord.Offset(origin);
                    var size = radius * 0.15f;

                    var location = center.Offset(new PointF(-size / 4, -size / 4));

                    e.Graphics.DrawString(hour.ToString(), DefaultFont, brush, new RectangleF(location, new SizeF(size, size)));
                }
            }
        }
    }
}