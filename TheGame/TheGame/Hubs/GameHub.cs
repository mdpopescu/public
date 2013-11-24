using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using TheGame.Models;

namespace TheGame.Hubs
{
  public class GameHub : Hub
  {
    public void Init(string gameId)
    {
      var game = GetGameByGameId(gameId);
      gamesByPlayerId[Context.ConnectionId] = game;
      game.AddPlayer(Context.ConnectionId);
      game.Refresh(Clients.Client(Context.ConnectionId));
    }

    public void Select(string id)
    {
      var game = GetGameByPlayerId(Context.ConnectionId);
      if (game == null)
        return;

      game.Toggle(Clients.Client, id);
    }

    //

    private static object gate = new object();
    private static Dictionary<string, Game> gamesByGameId = new Dictionary<string, Game>();
    private static Dictionary<string, Game> gamesByPlayerId = new Dictionary<string, Game>();

    private Game GetGameByGameId(string gameId)
    {
      lock (gate)
      {
        Game game;
        if (!gamesByGameId.TryGetValue(gameId, out game))
        {
          game = new Game(gameId);
          gamesByGameId.Add(gameId, game);
        }

        return game;
      }
    }

    private Game GetGameByPlayerId(string playerId)
    {
      lock (gate)
      {
        Game game;
        return gamesByPlayerId.TryGetValue(playerId, out game) ? game : null;
      }
    }
  }
}