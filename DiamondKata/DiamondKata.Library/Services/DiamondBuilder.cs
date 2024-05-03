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

        writer.WriteLine(ch.ToString());
    }
}