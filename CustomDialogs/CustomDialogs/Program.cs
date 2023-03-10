using System.Diagnostics;
using System.Windows.Automation;

namespace CustomDialogs;

internal class Program
{
    private static void Main(string[] args)
    {
        // we're going to look for a Notepad window
        var notepad = Process
            .GetProcessesByName("Notepad")
            .Select(it => it.MainWindowHandle)
            .Select(AutomationElement.FromHandle)
            .FirstOrDefault();
        if (notepad == null)
        {
            Console.WriteLine("Notepad not found.");
            return;
        }

        ListElements(notepad);
    }

    private static void ListElements(AutomationElement parent, int indent = 0)
    {
        var walker = TreeWalker.ControlViewWalker;

        var control = walker.GetFirstChild(parent);
        while (control != null)
        {
            Console.WriteLine(new string(' ', indent * 2) + control.Current.LocalizedControlType + ": " + control.Current.Name);
            ListElements(control, indent + 1);

            control = walker.GetNextSibling(control);
        }
    }
}