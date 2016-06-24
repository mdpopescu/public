using System;
using System.Threading;

namespace Renfield.Snake
{
  internal class Program
  {
    private const int FPS = 60;
    private static volatile bool gameOver;

    private static void Main(string[] args)
    {
      Console.CursorVisible = false;

      Events.Async = true;

      new ConsoleInput();
      new Snake();
      new RabbitGenerator();
      new Map();
      new Score();
      new Lives();
      new Sound();
      new Screen();

      Events.Subscribe(EventType.GameLost, (sender, e) => gameOver = true);

      var game = new Game();
      game.Initialize();

      while (!gameOver)
      {
        game.Next();
        Thread.Sleep(1000 / FPS);
      }

      Console.WriteLine("GAME OVER");
      Console.ReadLine();
    }
  }
}