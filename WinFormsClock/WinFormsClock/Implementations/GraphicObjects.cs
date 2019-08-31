using System;
using System.Drawing;
using WinFormsClock.Contracts;

namespace WinFormsClock.Implementations
{
    public class GraphicObjects : IGraphicObjects
    {
        public ICache<Brush, Color> Brushes { get; } = new Cache<Brush, Color>(10);
        public ICache<Pen, Tuple<Color, float>> Pens { get; } = new Cache<Pen, Tuple<Color, float>>(10);
        public ICache<Font, int> Fonts { get; } = new Cache<Font, int>(50);

        public void Dispose()
        {
            Brushes.Dispose();
            Pens.Dispose();
            Fonts.Dispose();
        }
    }
}