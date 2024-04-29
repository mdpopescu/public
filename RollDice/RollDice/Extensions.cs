namespace RollDice;

internal static class Extensions
{
    public static int ParseInt(this string s, int defValue = default) =>
        int.TryParse(s, out var result) ? result : defValue;
}