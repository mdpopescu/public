using System.Drawing;
using WinFormsClock.Contracts;

namespace WinFormsClock.Implementations
{
    internal class Background : IClockPart
    {
        public Color Color { get; set; } = Color.LightBlue;

        public void Draw(ICanvas canvas)
        {
            canvas.Clear(Color);
        }
    }
}