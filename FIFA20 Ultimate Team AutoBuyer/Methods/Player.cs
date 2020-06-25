using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FIFA20_Ultimate_Team_AutoBuyer.Methods
{
    public static class Player
    {
        private static readonly IEnumerable<Models.Filter> allPlayers;

        static Player()
        {
            using (StreamReader r = new StreamReader("players.json"))
            {

                string json = r.ReadToEnd();
                var response = JsonConvert.DeserializeObject<Models.NetworkPlayer>(json);

                allPlayers = response.Players.Select(p => new Models.Filter
                {
                    ID = p.id,
                    IsLegend = false,
                    PlayerName = string.IsNullOrEmpty(p.c) ? p.f + ' ' + p.l : p.c,
                    Rating = p.r
                }).Concat(response.LegendsPlayers.Select(p => new Models.Filter
                {
                    ID = p.id,
                    IsLegend = true,
                    PlayerName = string.IsNullOrEmpty(p.c) ? p.f + ' ' + p.l : p.c,
                    Rating = p.r
                }));
            }
        }

        public static int GetID(string playerName)
        {
            if (playerName == null) throw new ArgumentNullException("playerName");
            if (string.IsNullOrWhiteSpace(playerName)) throw new ArgumentException("playerName");
            return allPlayers.Where(player => string.Equals(player.PlayerName, playerName, StringComparison.OrdinalIgnoreCase)).Select(player => player.ID).FirstOrDefault();
        }

        public static int GetID(string playerName, int rating)
        {
            if (playerName == null) throw new ArgumentNullException("playerName");
            if (string.IsNullOrWhiteSpace(playerName)) throw new ArgumentException("playerName");
            if (rating <= 0) throw new ArgumentException("rating");
            return allPlayers.Where(player => player.Rating == rating && string.Equals(player.PlayerName, playerName, StringComparison.OrdinalIgnoreCase)).Select(player => player.ID).FirstOrDefault();
        }
            
        public static string GetName(int playerID)
        {
            if (playerID <= 0) throw new ArgumentException("playerID");
            return allPlayers.Where(player => player.ID == playerID).Select(player => player.PlayerName).FirstOrDefault();
        }

        public static int GetRating(int playerID)
        {
            if (playerID <= 0) throw new ArgumentException("playerID");
            return allPlayers.Where(player=> player.ID == playerID).Select(player => player.Rating).FirstOrDefault();
        }

        public static List<Models.Filter> GetAll()
        {
            return allPlayers.ToList();
        }
    }
}
