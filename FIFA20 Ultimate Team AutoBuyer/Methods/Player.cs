using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FIFA20_Ultimate_Team_AutoBuyer.Methods
{
    public static class Player
    {
        private static readonly IEnumerable<PlayerItem> allPlayers;

        static Player()
        {
            using (StreamReader r = new StreamReader("players.json"))
            {

                string json = r.ReadToEnd();
                var response = JsonConvert.DeserializeObject<Models.NetworkPlayer>(json);

                allPlayers = response.Players.Select(p => new PlayerItem
                {
                    Id = p.id,
                    IsLegend = false,
                    FriendlyName = string.IsNullOrEmpty(p.c) ? p.f + ' ' + p.l : p.c,
                    Rating = p.r
                }).Concat(response.LegendsPlayers.Select(p => new PlayerItem
                {
                    Id = p.id,
                    IsLegend = true,
                    FriendlyName = string.IsNullOrEmpty(p.c) ? p.f + ' ' + p.l : p.c,
                    Rating = p.r
                }));
            }
        }

        public static int GetID(string playerName)
        {
            if (playerName == null) throw new ArgumentNullException("playerName");
            if (string.IsNullOrWhiteSpace(playerName)) throw new ArgumentException("playerName");
            return allPlayers.Where(player => string.Equals(player.FriendlyName, playerName, StringComparison.OrdinalIgnoreCase)).Select(player => player.Id).FirstOrDefault();
        }

        public static int GetID(string playerName, int rating)
        {
            if (playerName == null) throw new ArgumentNullException("playerName");
            if (string.IsNullOrWhiteSpace(playerName)) throw new ArgumentException("playerName");
            if (rating <= 0) throw new ArgumentException("rating");
            return allPlayers.Where(player => player.Rating == rating && string.Equals(player.FriendlyName, playerName, StringComparison.OrdinalIgnoreCase)).Select(player => player.Id).FirstOrDefault();
        }
            
        public static string GetName(int playerID)
        {
            if (playerID <= 0) throw new ArgumentException("playerID");
            var playerName = allPlayers.Where(player => player.Id == playerID).Select(player => player.FriendlyName).FirstOrDefault();
            if (string.IsNullOrEmpty(playerName)) throw new Exception("Unable to resolve PlayerID");
            return playerName;
        }

        public static int GetRating(int playerID)
        {
            if (playerID <= 0) throw new ArgumentException("playerID");
            return allPlayers.Where(player=> player.Id == playerID).Select(player => player.Rating).FirstOrDefault();
        }

        public static List<PlayerItem> GetAll()
        {
            return allPlayers.ToList();
        }
    }
}
