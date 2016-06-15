using System;
using System.Net;
using System.Threading.Tasks;

namespace Renfield.AppUpdater.Helper
{
  internal class WebLoader : Loader
  {
    #region Implementation of Loader

    public event ProgressEventHandler Progress;
    public event CompletedEventHandler Completed;
    public event ErrorEventHandler Error;

    public void LoadInMemory(string uri)
    {
      using (var web = new WebClient())
      {
        if (Progress != null)
          web.DownloadProgressChanged += (sender, e) => Progress(e.ProgressPercentage);

        web.DownloadStringCompleted += (sender, e) =>
        {
          if (e.Error != null && Error != null)
            Error(e.Error);
          else if (Completed != null)
            Completed(e.Result);
        };

        Task.Factory.StartNew(() => web.DownloadStringAsync(new Uri(uri)));
      }
    }

    public void LoadToDisk(string uri, string fileName)
    {
      using (var web = new WebClient())
      {
        if (Progress != null)
          web.DownloadProgressChanged += (sender, e) => Progress(e.ProgressPercentage);

        web.DownloadFileCompleted += (sender, e) =>
        {
          if (e.Error != null && Error != null)
            Error(e.Error);
          else if (Completed != null)
            Completed(fileName);
        };

        web.DownloadFileAsync(new Uri(uri), fileName);
      }
    }

    #endregion
  }
}