using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TheGame.Models
{
  public class Game
  {
    public string Id { get; private set; }
    public List<string> Players { get; private set; }
    public Dictionary<string, bool> Selected { get; private set; }

    public Game(string id)
    {
      Id = id;
      Players = new List<string>();
      Selected = new Dictionary<string, bool>();
    }

    public void AddPlayer(string playerId)
    {
      if (!Players.Contains(playerId))
        Players.Add(playerId);
    }

    public void Refresh(dynamic client)
    {
      var ids = from kv in Selected
                where kv.Value
                select kv.Key;
      client.selectMany(ids.ToArray());
    }

    public void Toggle(Func<string, dynamic> clients, string id)
    {
      if (!Selected.ContainsKey(id))
        Selected.Add(id, false);
      Selected[id] = !Selected[id];

      foreach (var player in Players)
        clients(player).select(id);
    }
  }
}