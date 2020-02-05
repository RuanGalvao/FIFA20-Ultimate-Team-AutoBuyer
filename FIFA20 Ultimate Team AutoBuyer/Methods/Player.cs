using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FIFA20_Ultimate_Team_Autobuyer.Methods
{
    public static class Player
    {
        private static readonly Models.Player allPlayers;

        static Player()
        {
            using (StreamReader r = new StreamReader("players.json"))
            {
                string json = r.ReadToEnd();
                allPlayers = JsonConvert.DeserializeObject<Models.Player>(json);
            }
        }

        public static int GetPlayerID(string playerName)
        {
            return allPlayers.Players
                .Concat(allPlayers.LegendsPlayers)
                .Where(x => string.Equals(string.IsNullOrEmpty(x.c) ? x.f + ' ' + x.l : x.c, playerName, StringComparison.OrdinalIgnoreCase))
                .Select(player => player.id)
                .FirstOrDefault();
        }
            
        public static string GetPlayerName(int playerID)
        {
            return allPlayers.Players.Concat(allPlayers.LegendsPlayers).Where(x => x.id == playerID).Select(p => string.IsNullOrEmpty(p.c) ? p.f + ' ' + p.l : p.c).FirstOrDefault();
        }

        public static List<Models.Player.PlayerModel> ReturnAllPlayers()
        {
            return allPlayers.Players.Concat(allPlayers.LegendsPlayers).ToList();
        }
    }
}
