namespace Renfield.AppUpdater.Helper
{
  public interface Presenter
  {
    void Initialize();
    void SetState(AutoUpdaterState newState);
    void SetProgress(int percentage);
  }
}