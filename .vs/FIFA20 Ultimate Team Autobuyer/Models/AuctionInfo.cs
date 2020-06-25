using FIFA20_Ultimate_Team_AutoBuyer.Models;
using System.Collections.Generic;

namespace FIFA20_Ultimate_Team_AutoBuyer
{
    public class AuctionInfo
    {
        public IEnumerable<ItemModel> auctionInfo;

        public class ItemModel
        {
            public int BuyNowPrice { get; set; }
            public int CurrentBid { get; set; }
            public int Expires { get; set; }
            public ItemData ItemData { get; set; }
            public long TradeId { get; set; }
            public string TradeState { get; set; }
        }

        public int credits { get; set; }

    }
}
