using System;
using System.Collections.Generic;
using System.Linq;
using SocialNetwork.Library.Services;

namespace SocialNetwork
{
  internal static class Program
  {
    private static void Main(string[] args)
    {
      var handler = new CommandHandler(new InMemoryMessageRepository(), new InMemoryUserRepository());

      Console.WriteLine("Social Network");
      Console.WriteLine();

      while (true)
      {
        Console.Write("> ");

        var line = Console.ReadLine();
        if (line == null)
          break;

        var parts = line.Split(' ');
        var user = parts[0];

        if (parts.Length == 1)
          ShowLines(handler.Read(user));
        else if (parts[1] == "->")
          handler.Post(user, string.Join(" ", parts.Skip(2)));
        else if (parts[1] == "follows")
          handler.Follow(user, parts[2]);
        else if (parts[1] == "wall")
          ShowLines(handler.Wall(user));
      }
    }

    private static void ShowLines(IEnumerable<string> lines)
    {
      foreach (var line in lines)
        Console.WriteLine(line);
    }
  }
}