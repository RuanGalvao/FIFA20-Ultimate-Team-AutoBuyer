using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIFA20_Ultimate_Team_AutoBuyer.Models
{
    public class TradePile
    {
        public List<ItemData> itemData;
        public class ItemData
        {
            public long id { get; set; }
            public string pile { get; set; }
        }
    }
}
