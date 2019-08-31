using System.Drawing;

namespace WinFormsClock.Contracts
{
    internal interface IClockPart
    {
        Color Color { get; set; }

        void Draw(ICanvas canvas);
    }
}