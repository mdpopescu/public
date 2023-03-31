namespace Turtles.Library.Contracts;

public interface IFileManager
{
    string Text { get; set; }

    string? Filename { get; }
    bool IsModified { get; }

    /// <summary>
    ///     Tries to clear the filename and content.
    /// </summary>
    /// <returns><c>true</c> if the call was successful.</returns>
    /// <remarks>If the current file is modified, tries to save it; this might fail.</remarks>
    bool New();

    /// <summary>
    ///     Tries to open a file.
    /// </summary>
    /// <returns><c>true</c> if the call was successful.</returns>
    /// <remarks>If the current file is modified, tries to save it; this might fail.</remarks>
    bool Open();

    /// <summary>
    ///     Tries to save the current file.
    /// </summary>
    /// <returns><c>true</c> if the call was successful.</returns>
    /// <remarks>
    ///     If the current file is unnamed, tries to request the name from the UI; this might fail (the user might
    ///     cancel).
    /// </remarks>
    bool Save();

    /// <summary>
    ///     Tries to save the current file with a new name.
    /// </summary>
    /// <returns><c>true</c> if the call was successful.</returns>
    /// <remarks>
    ///     Tries to request the name from the UI; this might fail (the user might cancel).
    /// </remarks>
    bool SaveAs();
}