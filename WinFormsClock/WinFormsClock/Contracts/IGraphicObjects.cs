using System;
using System.Drawing;

namespace WinFormsClock.Contracts
{
    public interface IGraphicObjects
    {
        ICache<Color, Brush> Brushes { get; }
        ICache<Tuple<Color, float>, Pen> Pens { get; }
        ICache<int, Font> Fonts { get; }
    }
}