using System.Drawing;
using System.Linq;
using WinFormsClock.Contracts;

namespace WinFormsClock.Implementations
{
    internal class MinuteMarks : IClockPart
    {
        public Color Color { get; set; } = Color.Black;

        public void Draw(ICanvas canvas)
        {
            foreach (var minute in Enumerable.Range(0, 60))
            {
                var degree = minute * 6.0f;

                canvas.Line(Color, 1.0f, canvas.Point(degree, 0.90f), canvas.Point(degree, 0.95f));
            }
        }
    }
}