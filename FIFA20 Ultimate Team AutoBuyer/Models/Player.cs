using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIFA20_Ultimate_Team_Autobuyer.Models
{
    public class Player
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
