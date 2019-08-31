using System;
using System.Drawing;

namespace WinFormsClock.Contracts
{
    public interface IGraphicObjects : IDisposable
    {
        ICache<Brush, Color> Brushes { get; }
        ICache<Pen, Tuple<Color, float>> Pens { get; }
        ICache<Font, int> Fonts { get; }
    }
}