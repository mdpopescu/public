namespace Renfield.AppUpdater.Helper
{
  public interface Loader
  {
    event ProgressEventHandler Progress;
    event CompletedEventHandler Completed;
    event ErrorEventHandler Error;

    /// <summary>
    ///   Asynchronously loads a resource, calling Completed with the result or Error if there's a problem
    /// </summary>
    /// <param name = "uri">URI of the resource to load</param>
    void LoadInMemory(string uri);

    /// <summary>
    ///   Asynchronously loads a resource and saves it to a file, calling Progress as downloading and Completed with the file name or Error if there's a problem
    /// </summary>
    /// <param name = "uri">URI of the resource to load</param>
    /// <param name = "fileName">Name of the file where the resource will be saved</param>
    void LoadToDisk(string uri, string fileName);
  }
}