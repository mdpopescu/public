using System;
using System.Drawing;
using System.Windows.Forms;
using Sierpinski.Models;
using Sierpinski.Services;

namespace Sierpinski
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            canvas = new Canvas(pictureBox1);
        }

        //

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            foreach (var point in triangle.Points)
                canvas.DrawPoint(point);

            current = triangle.GenerateRandomPointInside();

            timer1.Enabled = true;
        }

        //

        // NOTE: Origin (0, 0) is top left, X grows to the right, Y grows downwards

        private readonly Triangle triangle = new();

        private readonly Canvas canvas;

        private Point current;

        private int count;

        private static Point PickMidway(Point a, Point b) =>
            new((a.X + b.X) / 2, (a.Y + b.Y) / 2);

        //

        private void timer1_Tick(object sender, EventArgs e)
        {
            var point = triangle.PickRandomVertex();
            current = PickMidway(current, point);
            canvas.DrawPoint(current);

            Text = $"Points drawn: {++count}";
        }
    }
}