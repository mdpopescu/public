namespace Renfield.Snake
{
  public enum EventType
  {
    InitializeObjects,
    CollisionDetected,
    ScoreChanged,
    Create,
    Tick,
    Moving,
    Moved,
    NewBorn,
    MapChanged,
    LeftPressed,
    RightPressed,
    UpPressed,
    DownPressed,
    ObstacleHit,
    LivesChanged,
    GameLost
  }
}