using System;

namespace Tester.Models
{
    public class Notification
    {
        public int Row { get; }
        public int Col { get; }
        public string Text { get; }

        public ConsoleColor Background { get; set; }
        public ConsoleColor Foreground { get; set; }

        public Notification(int row, int col, string text)
        {
            Row = row;
            Col = col;
            Text = text;

            Background = ConsoleColor.Black;
            Foreground = ConsoleColor.White;
        }
    }
}