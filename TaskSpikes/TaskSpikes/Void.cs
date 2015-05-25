namespace TaskSpikes
{
  public struct Void
  {
    public static Void Singleton
    {
      get { return SINGLETON; }
    }

    //

    private static readonly Void SINGLETON = new Void();
  }
}