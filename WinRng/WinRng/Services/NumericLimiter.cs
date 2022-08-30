using System.Globalization;

namespace WinRng.Services;

public class NumericLimiter
{
    public NumericLimiter()
    {
        // Assumption: the decimal separator is exactly one character
        sep = CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator[0];
    }

    /// <summary>
    ///     Returns <c>true</c> if the character is allowed, <c>false</c> otherwise.
    /// </summary>
    /// <param name="text">The current text in the textbox.</param>
    /// <param name="newChar">The new char that has just been entered.</param>
    /// <returns><c>true</c> if the character is allowed, <c>false</c> otherwise.</returns>
    public bool AllowChar(string text, char newChar)
    {
        // Allow digits and backspace
        if (newChar is >= '0' and <= '9')
            return true;

        if (newChar == BACKSPACE)
            return true;

        // Disallow anything besides digits and the decimal separator
        if (newChar != sep)
            return false;

        // Only allow the decimal separator if not already present
        return !text.Contains(sep);
    }

    //

    private const int BACKSPACE = 0x08;

    private readonly char sep;
}