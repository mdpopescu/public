using System;

namespace Renfield.AppUpdater.Helper
{
  public delegate void ProgressEventHandler(int percentage);
  public delegate void CompletedEventHandler(string result);
  public delegate void ErrorEventHandler(Exception exception);
}