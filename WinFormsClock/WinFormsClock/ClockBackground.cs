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

        //

        private void ClockBackground_Load(object sender, EventArgs e)
        {
            //
        }

        private void ClockBackground_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.LightBlue);

            // draw the minute marks
            var origin = new Point(e.ClipRectangle.Width / 2, e.ClipRectangle.Height / 2);
            var radius = Math.Min(origin.X, origin.Y);

            using (var pen = new Pen(Color.Black))
            {
                foreach (var minute in Enumerable.Range(0, 60))
                {
                    var degree = minute * 6;

                    var lineStart = PolarCoord.Create(degree, radius * 0.90f).CarthesianCoord.Offset(origin);
                    var lineEnd = PolarCoord.Create(degree, radius * 0.95f).CarthesianCoord.Offset(origin);

                    e.Graphics.DrawLine(pen, lineStart.X, lineStart.Y, lineEnd.X, lineEnd.Y);
                }
            }
        }
    }
}