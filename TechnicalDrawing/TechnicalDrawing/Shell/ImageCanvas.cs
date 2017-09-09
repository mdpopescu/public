using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

        public void Execute(IEnumerable<ProjectedCommand> commands)
        {
            var groups = commands.GroupBy(it => it.Plane);
            foreach (var group in groups)
            {
                var picture = quadrants[(int) group.Key];

                RunBatch(picture, group);

                picture.Invalidate();
            }
        }

        //

        private readonly PictureBox[] quadrants;

        private static void Initialize(PictureBox picture)
        {
            picture.Image?.Dispose();
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

        private static void RunBatch(PictureBox picture, IEnumerable<ProjectedCommand> commands)
        {
            using (var g = Graphics.FromImage(picture.Image))
            using (var pen = new Pen(Color.Black, 1))
            {
                foreach (var command in commands)
                {
                    // only lines are implemented for now
                    g.DrawLine(
                        pen,
                        command.Points[0].X,
                        picture.Height - command.Points[0].Y,
                        command.Points[1].X,
                        picture.Height - command.Points[1].Y);
                }
            }
        }
    }
}