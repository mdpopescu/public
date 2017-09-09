using System.Drawing;
using TechnicalDrawing.Library.Contracts;
using TechnicalDrawing.Library.Models;

namespace TechnicalDrawing.Shell
{
    public class ImageCanvas : Canvas
    {
        public ImageCanvas(Image xyQuadrant, Image xzQuadrant, Image yzQuadrant)
        {
            quadrants = new[] { xyQuadrant, xzQuadrant, yzQuadrant };
        }

        public void Execute(ProjectedCommand command)
        {
            // only lines are implemented for now

            using (var g = Graphics.FromImage(quadrants[(int) command.Plane]))
            using (var pen = new Pen(Color.Black, 1.0f))
                g.DrawLine(pen, command.Points[0].X, command.Points[0].Y, command.Points[1].X, command.Points[1].Y);
        }

        //

        private readonly Image[] quadrants;
    }
}