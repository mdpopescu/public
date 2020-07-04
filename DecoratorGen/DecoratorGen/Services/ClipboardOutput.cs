using DecoratorGen.Library.Contracts;
using TextCopy;

namespace DecoratorGen.Services
{
    public class ClipboardOutput : IOutput
    {
        public void WriteCode(string code) => CLIPBOARD.SetText(code);

        //

        private static readonly Clipboard CLIPBOARD = new Clipboard();
    }
}