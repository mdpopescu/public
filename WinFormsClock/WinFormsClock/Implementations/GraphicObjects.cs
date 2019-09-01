using System;
using System.Drawing;
using WinFormsClock.Contracts;

namespace WinFormsClock.Implementations
{
    public class GraphicObjects : IGraphicObjects, IDisposable
    {
        public ICache<Color, Brush> Brushes { get; } = new Cache<Color, Brush>(10);
        public ICache<Tuple<Color, float>, Pen> Pens { get; } = new Cache<Tuple<Color, float>, Pen>(10);
        public ICache<int, Font> Fonts { get; } = new Cache<int, Font>(50);

        public void Dispose()
        {
            Brushes.Dispose();
            Pens.Dispose();
            Fonts.Dispose();
        }
    }
}