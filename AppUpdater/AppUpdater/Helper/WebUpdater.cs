using System;
using System.IO;

namespace Renfield.AppUpdater.Helper
{
  internal class WebUpdater : Updater
  {
    private readonly Presenter presenter;
    private readonly Func<Loader> loaderFactory;
    private string URL;

    public WebUpdater(Presenter presenter, Func<Loader> loaderFactory)
    {
      this.presenter = presenter;
      this.loaderFactory = loaderFactory;
    }

    #region Implementation of Updater

    public void CheckForUpdate(string uri, string currentVersion, ManifestReader reader)
    {
      presenter.SetState(AutoUpdaterState.Checking);

      var loader = loaderFactory.Invoke();
      loader.Error += ex => presenter.SetState(AutoUpdaterState.Error);
      loader.Completed += manifest =>
      {
        reader.Read(manifest);
        URL = reader.URL;

        presenter.SetState(currentVersion != reader.Version ? AutoUpdaterState.UpdateExists : AutoUpdaterState.Ok);
      };

      loader.LoadInMemory(uri);
    }

    public void Update(string destinationFolder, System system)
    {
      presenter.SetState(AutoUpdaterState.Updating);

      var fileName = Path.Combine(destinationFolder, Path.GetFileName(new Uri(URL).LocalPath));

      var loader = loaderFactory.Invoke();
      loader.Error += ex => presenter.SetState(AutoUpdaterState.Error);
      loader.Completed += name =>
      {
        presenter.SetState(AutoUpdaterState.Ok);
        system.Launch(name);
        system.EndCurrentApplication();
      };
      loader.Progress += percentage => presenter.SetProgress(percentage);

      loader.LoadToDisk(URL, fileName);
    }

    #endregion
  }
}