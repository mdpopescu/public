using System;

namespace Statix.Library.Models
{
    public class MarkdownFile
    {
        public string Filename { get; set; }
        public DateTimeOffset Date { get; set; }
        public string Content { get; set; }
    }
}