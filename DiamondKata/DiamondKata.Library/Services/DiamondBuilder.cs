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
        // equal to CH-'A'+1

        var n = ch - 'A' + 1;
        var len = n * 2 - 1;

        // the result is a matrix of len x len characters, initialized to spaces
        var result = Enumerable
            .Range(1, len)
            .Select(_ => Enumerable.Range(1, len).Select(_ => ' ').ToArray())
            .ToArray();

        // 'A' must be on rows 0 and LEN-1 -- current-'A' and LEN-(current-'A')-1
        // 'B' must be on rows 1 and LEN-2 -- current-'A' and LEN-(current-'A')-1
        // ...
        // CH  must be on row N-1 which is equal to LEN/2

        // 'A' must be on column N-1          -- N-(current-'A')-1 and N+(current-'A')-1
        // 'B' must be on columns N-2 and N   -- N-(current-'A')-1 and N+(current-'A')-1
        // 'C' must be on columns N-3 and N+1 -- N-(current-'A')-1 and N+(current-'A')-1
        // ...

        // the zero-indexing makes formulas more complicated than they should be

        for (var current = 'A'; current <= ch; current++)
        {
            var diff = current - 'A';

            result[diff][n - diff - 1] = current;
            result[diff][n + diff - 1] = current;
            result[len - diff - 1][n - diff - 1] = current;
            result[len - diff - 1][n + diff - 1] = current;
        }

        foreach (var row in result)
            writer.WriteLine(new string(row));
    }
}