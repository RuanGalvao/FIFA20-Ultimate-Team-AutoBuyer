using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FIFA20_Ultimate_Team_Autobuyer.Methods
{
    public static class Player
    {
        private static readonly IEnumerable<Models.InternalPlayer> allPlayers;

        static Player()
        {
            using (StreamReader r = new StreamReader("players.json"))
            {

                string json = r.ReadToEnd();
                var response = JsonConvert.DeserializeObject<Models.NetworkPlayer>(json);

                allPlayers = response.Players.Select(p => new Models.InternalPlayer
                {
                    ID = p.id,
                    IsLegend = false,
                    Name = string.IsNullOrEmpty(p.c) ? p.f + ' ' + p.l : p.c,
                    Rating = p.r
                }).Concat(response.LegendsPlayers.Select(p => new Models.InternalPlayer
                {
                    ID = p.id,
                    IsLegend = true,
                    Name = string.IsNullOrEmpty(p.c) ? p.f + ' ' + p.l : p.c,
                    Rating = p.r
                }));
            }
        }

        public static int GetPlayerID(string playerName)
        {
            return allPlayers.Where(player => string.Equals(player.Name, playerName, StringComparison.OrdinalIgnoreCase)).Select(player => player.ID).FirstOrDefault();
        }
            
        public static string GetPlayerName(int playerID)
        {
            return allPlayers.Where(player => player.ID == playerID).Select(player => player.Name).FirstOrDefault();
        }

        public static int GetPlayerRating(int playerID)
        {
            return allPlayers.Where(player=> player.ID == playerID).Select(player => player.Rating).FirstOrDefault();
        }

        public static List<Models.InternalPlayer> ReturnAllPlayers()
        {
            return allPlayers.ToList();
        }
    }
}
