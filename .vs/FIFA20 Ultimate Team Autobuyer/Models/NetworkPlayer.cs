using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIFA20_Ultimate_Team_AutoBuyer.Models
{
    public class NetworkPlayer
    {

        public IEnumerable<PlayerModel> LegendsPlayers;

        public IEnumerable<PlayerModel> Players;

        public class PlayerModel
        {
            public string f;
            public int id;
            public string l;
            public int r;
            public string c;
        }
    }
}
