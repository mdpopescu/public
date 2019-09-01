using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using WinFormsClock.Contracts;
using WinFormsClock.Implementations;

namespace WinFormsClock
{
    public partial class Clock : UserControl
    {
        public Clock()
        {
            InitializeComponent();

            parts = new IClockPart[]
            {
                new Background { Color = BACKGROUND },
                new MinuteMarks { Color = MARKS },
                new Numbers { Color = HOURS },
                new HandsOrigin { Color = HANDS },
                new HoursHand { Color = HANDS, Width = HOUR_WIDTH },
                new MinutesHand { Color = HANDS, Width = MINUTE_WIDTH },
                new SecondsHand { Color = HANDS, Width = SECOND_WIDTH },
            };

            graphicObjects = new GraphicObjects();
            Disposed += (sender, e) => graphicObjects.Dispose();
        }

        //

        private const float HOUR_WIDTH = 6.0f;
        private const float MINUTE_WIDTH = 3.0f;
        private const float SECOND_WIDTH = 1.0f;

        private static readonly Color BACKGROUND = Color.LightBlue;
        private static readonly Color MARKS = Color.Black;
        private static readonly Color HOURS = Color.Red;
        private static readonly Color HANDS = Color.BlueViolet;

        private readonly IEnumerable<IClockPart> parts;
        private readonly GraphicObjects graphicObjects;

        private Canvas CreateCanvas(PaintEventArgs e)
        {
            var origin = new Point(e.ClipRectangle.Width / 2, e.ClipRectangle.Height / 2);
            var size = Math.Min(origin.X, origin.Y);

            var g = new GraphicsAdapter(graphicObjects, e.Graphics);
            return new Canvas(g, origin, size);
        }

        //

        private void Clock_Load(object sender, EventArgs e)
        {
            //
        }

        private void Clock_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var canvas = CreateCanvas(e);
            foreach (var part in parts)
                part.Draw(canvas);
        }
    }
}