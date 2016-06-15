using Renfield.AppUpdater.Properties;

namespace Renfield.AppUpdater.Helper
{
  internal class AutoUpdaterPresenter : Presenter
  {
    private readonly AutoUpdaterView view;
    private AutoUpdaterState state;

    public AutoUpdaterPresenter(AutoUpdaterView view)
    {
      this.view = view;
    }

    #region Implementation of Presenter

    public void Initialize()
    {
      view.SetImage(Resources.ok);
      view.SetToolTip(Resources.AppStatusOk);
      view.SetCheckVisible(true);
      view.SetUpdateVisible(false);
      view.SetProgressVisible(false);
    }

    public void SetState(AutoUpdaterState newState)
    {
      if (state != newState && state == AutoUpdaterState.Updating)
        HideProgressBar();
      state = newState;

      switch (newState)
      {
        case AutoUpdaterState.Ok:
          view.SetImage(Resources.ok);
          view.SetToolTip(Resources.AppStatusOk);
          view.SetCheckVisible(true);
          view.SetUpdateVisible(false);
          break;

        case AutoUpdaterState.Error:
          view.SetImage(Resources.exclamation);
          view.SetToolTip(Resources.AppStatusError);
          view.SetCheckVisible(true);
          view.SetUpdateVisible(false);
          break;

        case AutoUpdaterState.UpdateExists:
          view.SetImage(Resources.information);
          view.SetToolTip(Resources.AppStatusUpdateFound);
          view.SetCheckVisible(false);
          view.SetUpdateVisible(true);
          break;

        case AutoUpdaterState.Checking:
          view.SetImage(Resources.clock_network);
          view.SetToolTip(Resources.AppStatusChecking);
          view.SetCheckVisible(false);
          view.SetUpdateVisible(false);
          break;

        case AutoUpdaterState.Updating:
          view.SetImage(Resources.clock_network);
          view.SetToolTip(Resources.AppStatusDownloading);
          view.SetCheckVisible(false);
          view.SetUpdateVisible(false);

          ShowProgressBar();
          break;
      }
    }

    public void SetProgress(int percentage)
    {
      view.SetProgressValue(percentage);
    }

    #endregion

    //

    private void HideProgressBar()
    {
      view.SetProgressVisible(false);
      view.Shrink();

      if (view.IsRightAnchored)
        view.MoveRight();
    }

    private void ShowProgressBar()
    {
      view.SetProgressVisible(true);
      view.Enlarge();

      if (view.IsRightAnchored)
        view.MoveLeft();
    }
  }
}