using System.Drawing;
using System.Linq;
using WinFormsClock.Contracts;

namespace WinFormsClock.Implementations
{
    internal class Numbers : IClockPart
    {
        public Color Color { get; set; } = Color.Red;

        public void Draw(ICanvas canvas)
        {
            foreach (var hour in Enumerable.Range(0, 12))
            {
                var degree = hour * 30;
                var sHour = hour == 0 ? "12" : hour.ToString(); // hour 0 should be 12

                canvas.DrawString(Color, canvas.Square(canvas.Point(degree, 0.85f), 0.15f), sHour);
            }
        }
    }
}