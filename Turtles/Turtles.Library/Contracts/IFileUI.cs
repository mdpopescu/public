using Turtles.Library.Models;

namespace Turtles.Library.Contracts;

public interface IFileUI
{
    string? GetFilenameToOpen();
    string? GetFilenameToSave();
    ConfirmationResponse ConfirmSave();
}