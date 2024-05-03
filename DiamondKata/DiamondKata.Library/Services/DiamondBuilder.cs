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

        // taking spaces into account, the result will be a square
        // the length of the square would be N*2-1, with N being
        // equal to ch-'A'+1
        // if the current line is I, with I starting at 0,

        var n = ch - 'A' + 1;
        var len = n * 2 - 1;

        // the result is a matrix of len x len characters, initialized to spaces
        var result = Enumerable
            .Range(1, len)
            .Select(_ => Enumerable.Range(1, len).Select(_ => ' ').ToArray())
            .ToArray();

        // TODO: fill out the correct letters

        foreach (var row in result)
            writer.WriteLine(new string(row));
    }
}