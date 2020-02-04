using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIFA20_Ultimate_Team_Autobuyer.Models
{
    public class Sell
    {
        public ItemData itemData;

        public class ItemData
        {
            public long id;
        }

        public int startingBid { get; set; }
        public int duration { get; set; }
        public int buyNowPrice { get; set; }
    }
}
