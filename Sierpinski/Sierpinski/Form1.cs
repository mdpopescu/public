using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sierpinski.Models;

namespace Sierpinski
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            points.AddRange(triangle.Points);

            current = triangle.GenerateRandomPointInside();

            addPointTimer.Enabled = true;
        }

        //

        // NOTE: Origin (0, 0) is top left, X grows to the right, Y grows downwards

        private readonly Pen pen = new(Color.Black);

        private readonly Triangle triangle = new();
        private readonly List<Point> points = new();

        private Point current;

        private static Point PickMidway(Point a, Point b) =>
            new((a.X + b.X) / 2, (a.Y + b.Y) / 2);

        //

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            var rect = e.ClipRectangle;
            var scaleX = rect.Width / (float)Extensions.MAX_X;
            var scaleY = rect.Height / (float)Extensions.MAX_Y;

            Point ToCanvas(Point p) =>
                new((int)Math.Round(p.X * scaleX), (int)Math.Round(p.Y * scaleY));

            var g = e.Graphics;
            foreach (var canvasPoint in points.Select(ToCanvas))
                g.DrawEllipse(pen, canvasPoint.X, canvasPoint.Y, 2, 2);
        }

        private void addPointTimer_Tick(object sender, EventArgs e)
        {
            var point = triangle.PickRandomVertex();
            current = PickMidway(current, point);
            points.Add(current);
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            pictureBox1.Invalidate();
            Text = $"Points drawn: {points.Count}";
        }
    }
}