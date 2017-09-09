using System.Drawing;
using System.Windows.Forms;
using TechnicalDrawing.Library.Contracts;
using TechnicalDrawing.Library.Models;

namespace TechnicalDrawing.Shell
{
    public class ImageCanvas : Canvas
    {
        public ImageCanvas(PictureBox xyQuadrant, PictureBox xzQuadrant, PictureBox yzQuadrant)
        {
            // create the blank images
            Initialize(xyQuadrant);
            Initialize(xzQuadrant);
            Initialize(yzQuadrant);

            quadrants = new[] { xyQuadrant, xzQuadrant, yzQuadrant };
        }

        public void Execute(ProjectedCommand command)
        {
            var picture = quadrants[(int) command.Plane];

            // only lines are implemented for now
            using (var g = Graphics.FromImage(picture.Image))
            using (var pen = new Pen(Color.Black, 1))
                g.DrawLine(pen, command.Points[0].X, command.Points[0].Y, command.Points[1].X, command.Points[1].Y);

            picture.Invalidate();
        }

        //

        private readonly PictureBox[] quadrants;

        private void Initialize(PictureBox picture)
        {
            picture.Image = CreateImage(picture.Width, picture.Height, Color.White);
        }

        private static Image CreateImage(int width, int height, Color color)
        {
            var image = new Bitmap(width, height);

            using (var g = Graphics.FromImage(image))
            using (var b = new SolidBrush(color))
                g.FillRectangle(b, 0, 0, image.Width, image.Height);

            return image;
        }
    }
}