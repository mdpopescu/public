namespace DiamondKata.Library.Services;

public class DiamondBuilder
{
    public void Build(string[] args, TextWriter writer)
    {
        if (args.Length != 1)
            return;

        var arg = args[0].ToUpperInvariant();
        if (arg.Length != 1)
            return;

        var ch = arg[0];
        if (ch is < 'A' or > 'Z')
            return;

        // do the simplest thing that works
        if (ch == 'A')
        {
            writer.WriteLine("A");
        }
        else
        {
            writer.WriteLine(" A ");
            writer.WriteLine("B B");
            writer.WriteLine(" A ");
        }
    }
}