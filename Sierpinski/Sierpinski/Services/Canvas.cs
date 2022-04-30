using System;
using System.Drawing;
using System.Windows.Forms;

namespace Sierpinski.Services
{
    public class Canvas
    {
        public Canvas(Control control)
        {
            g = control.CreateGraphics();
            scaleX = control.Width / 100.0f;
            scaleY = control.Height / 100.0f;
        }

        public void DrawPoint(Point p)
        {
            var canvasPoint = new Point((int)Math.Round(p.X * scaleX), (int)Math.Round(p.Y * scaleY));
            g.DrawEllipse(pen, canvasPoint.X, canvasPoint.Y, 2, 2);
        }

        //

        private readonly Pen pen = new(Color.Black);

        private readonly Graphics g;
        private readonly float scaleX, scaleY;
    }
}