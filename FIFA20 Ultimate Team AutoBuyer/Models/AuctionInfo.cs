using FIFA20_Ultimate_Team_Autobuyer.Models;
using System.Collections.Generic;

namespace FIFA20_Ultimate_Team_Autobuyer
{
    public class AuctionInfo
    {
        public IEnumerable<ItemModel> auctionInfo;

        public class ItemModel
        {
            public string BidState { get; set; }
            public int BuyNowPrice { get; set; }
            public int CurrentBid { get; set; }
            public int Expires { get; set; }
            public ItemData ItemData { get; set; }
            public int Offers { get; set; }
            public string SellerEstablished { get; set; }
            public int SellerId { get; set; }
            public bool? TradeOwner { get; set; }
            public string SellerName { get; set; }
            public int StartingBid { get; set; }
            public byte ConfidenceValue { get; set; }
            public long TradeId { get; set; }
            public string TradeIdStr { get; set; }
            public string TradeState { get; set; }
            public bool? Watched { get; set; }
            public int? CoinsProcessed { get; set; }
        }

        public int credits { get; set; }

    }
}
