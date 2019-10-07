using System.IO;
using PipesAndFilters.Contracts;

namespace PipesAndFilters.Models
{
    public class TextOutput : IEffect
    {
        public readonly string Text;

        public TextOutput(TextWriter writer, string text)
        {
            this.writer = writer;
            Text = text;
        }

        public void Execute() => writer.WriteLine(Text);

        //

        private readonly TextWriter writer;
    }
}