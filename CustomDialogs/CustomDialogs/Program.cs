using System.Diagnostics;
using System.Windows.Automation;

namespace CustomDialogs;

internal class Program
{
    private static void Main(string[] args)
    {
        // we're going to look for a Notepad window
        var notepadProcess = Process.GetProcessesByName("Notepad").FirstOrDefault();
        if (notepadProcess == null)
        {
            Console.WriteLine("Notepad not found.");
            return;
        }

        //var notepad = AutomationElement.FromHandle(notepadProcess.MainWindowHandle);

        //var walker = TreeWalker.ControlViewWalker;

        //var list = new List<AutomationElement>();

        //void OnItem(AutomationElement control, int spacing)
        //{
        //    list.Add(control);
        //    Console.WriteLine(new string(' ', spacing) + control.Current.LocalizedControlType + ": " + control.Current.Name);
        //}

        //ListElements(walker, notepad, 0, OnItem);

        ////var fileMenu = list.Where(it => it.Current.AutomationId == "2").FirstOrDefault();
        ////var fileOpenClicked = AutomationEvent.LookupById(2);

        //Automation.AddAutomationEventHandler(
        //    InvokePattern.InvokedEvent, //AutomationEvent.LookupById(2),
        //    AutomationElement.RootElement,
        //    TreeScope.Descendants,
        //    (src, e) =>
        //    {
        //        if (!Equals(e.EventId, InvokePattern.InvokedEvent))
        //            return;

        //        AutomationElement? sourceElement;
        //        string? name;
        //        try
        //        {
        //            sourceElement = src as AutomationElement;
        //            name = sourceElement?.Current.Name;
        //        }
        //        catch (ElementNotAvailableException)
        //        {
        //            return;
        //        }

        //        Console.WriteLine($"{e.EventId.Id} detected: {name}.");

        //        if (name != null)
        //        {
        //            Console.WriteLine(name == "Open" ? "That was a File/Open." : "That was something else.");
        //        }


        //        //if (e.EventId.Equals(AutomationEvent.LookupById(2)))
        //        //    Console.WriteLine("File/Open invoked.");
        //    }
        //);

        Console.ReadLine();
    }

    private static void ListElements(TreeWalker walker, AutomationElement parent, int indent, Action<AutomationElement, int> callback)
    {
        var control = walker.GetFirstChild(parent);
        while (control != null)
        {
            callback.Invoke(control, indent * 2);
            ListElements(walker, control, indent + 1, callback);

            control = walker.GetNextSibling(control);
        }
    }
}