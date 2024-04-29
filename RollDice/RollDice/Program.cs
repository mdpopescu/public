using System.Text.RegularExpressions;
using RollDice;

const string HELP = """
                    rolldice /?         Shows this help text
                    rolldice [spec]...  Generates a number of random numbers according to the given
                                        restrictions. In particular, rolldice without any arguments
                                        will be equivalent to rolldice 1d6
                        spec            The dice specifications, of the form <m>d<n>[+<k>], where
                            m               The number of dice to be rolled, optional, defaults to 1
                            n               The upper limit of the dice, optional, defaults to 6
                            k               The value to add to the result, optional, defaults to 0
                                            NOTE: the k value must be present if the + sign appears
                    """;

var re = new Regex(@"(\d*)d(\d*)(\+(\d+))?", RegexOptions.Compiled);

if (args is ["/?"])
{
    Console.WriteLine(HELP);
    return;
}

var results = from arg in args.DefaultIfEmpty("1d6+0")
              let match = re.Match(arg)
              where match.Success
              let value = GenerateRandom(match)
              select $"{arg}: {value}";

Console.WriteLine(string.Join(" ", results));

return;

int GenerateRandom(Match match)
{
    var m = match.Groups[1].Value.ParseInt(1);
    var n = match.Groups[2].Value.ParseInt(6);
    var k = match.Groups[4].Value.ParseInt(0);

    return Enumerable.Range(1, m).Select(_ => Random.Shared.Next(n) + 1).Sum() + k;
}