namespace CommandLineRouter;

public static class Router
{
    /// <summary>
    ///     Registers an action to be executed if the given <paramref name="pattern" /> is detected.
    /// </summary>
    /// <param name="args">The command-line arguments.</param>
    /// <param name="pattern">The pattern to be handled.</param>
    /// <param name="handler">The action to be executed.</param>
    /// <remarks>
    ///     The <paramref name="handler" /> will receive the arguments that have been discovered, with the correct types. For
    ///     example, if the pattern is /d:{date:Date}, the handler will receive two objects, the string "d" and the Date object
    ///     representing the given date.
    ///     The '/' argument prefix can be replaced by '-' or even '--' for multi-character arguments (for example,
    ///     --date:{date:Date}).
    ///     The ':' separator can be replaced by '=' or ' ' (space).
    /// </remarks>
    public static void AddHandler(string[] args, string pattern, Action<object[]> handler)
    {
        throw new NotImplementedException();
    }
}