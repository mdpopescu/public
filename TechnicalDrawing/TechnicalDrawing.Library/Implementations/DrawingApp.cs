using TechnicalDrawing.Library.Contracts;

namespace TechnicalDrawing.Library.Implementations
{
    public class DrawingApp
    {
        public DrawingApp(Canvas canvas)
        {
            this.canvas = canvas;
        }

        public void Load(string filename)
        {
            //
        }

        //

        private Canvas canvas;
    }
}