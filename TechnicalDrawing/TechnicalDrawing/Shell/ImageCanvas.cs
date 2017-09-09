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
            var image = quadrants[(int) command.Plane];

            // only lines are implemented for now
            using (var g = Graphics.FromImage(image))
            using (var pen = new Pen(Color.Black, 1))
                g.DrawLine(pen, command.Points[0].X, command.Points[0].Y, command.Points[1].X, command.Points[1].Y);
        }

        //

        private readonly Image[] quadrants;
    }
}