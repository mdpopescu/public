using System.Drawing;

namespace FastRead.Services
{
    public class GraphicsCalculator
    {
        public Point GetCenterPosition(Size formSize, Size controlSize)
        {
            return new Point((formSize.Width - controlSize.Width) / 2, (formSize.Height - controlSize.Height) / 2);
        }
    }
}