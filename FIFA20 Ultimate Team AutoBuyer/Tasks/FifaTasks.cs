using FIFA20_Ultimate_Team_AutoBuyer.Methods;
using FIFA20_Ultimate_Team_AutoBuyer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FIFA20_Ultimate_Team_AutoBuyer.Tasks
{
    public class FifaTasks 
    {
        private readonly viewModel ViewModel;
        private readonly NetworkTasks NetworkTasks;
        private readonly string BaseURL = $"https://utas.external.s3.fut.ea.com/ut/game/fifa{Declarations.YEAR}/";

        private PlayerURL PlayerURL;
        private ChemistryStyleURL ChemistryStyleURL;

        public FifaTasks(viewModel viewModel)
        {
            ViewModel = viewModel;
            NetworkTasks = new NetworkTasks(viewModel);
        }

        private int CalculateActiveListingsTotalValue(IEnumerable<NetworkAuctionInfo.ItemModel> items)
        {
            return items.Where(i => i.Expires > 0).Sum(item => item.BuyNowPrice);
        }

        private int CalculateExpiredListingsTotalValue(IEnumerable<NetworkAuctionInfo.ItemModel> items)
        {
            return items.Where(i => i.Expires == -1 && i.CurrentBid == 0).Sum(item => item.BuyNowPrice);
        }

        private async Task<int> CalculateUnlistedListingsTotalValueAsync(IEnumerable<NetworkAuctionInfo.ItemModel> items)
        {
            var unlistedItems = items.Where(i => i.Expires == 0);
            var runningTotal = 0;
            foreach (var item in unlistedItems)
            {
                Sleep();
                runningTotal += await GetPlayerSellingPriceUsingResourceID(item.ItemData.ResourceId);
            }
            return runningTotal;
        }

        public async Task<int> CalculateTradepileAssetsTotalValue(IEnumerable<NetworkAuctionInfo.ItemModel> items)
        {
            if (items == null) return 0;
            var total = CalculateActiveListingsTotalValue(items);
            total += CalculateExpiredListingsTotalValue(items);
            total += await CalculateUnlistedListingsTotalValueAsync(items);
            return (int)(total * 0.95);
        }

        public async Task<int> GetPlayerSellingPriceUsingResourceID(long resourceId)
        {
            var response = await NetworkTasks.Get($"{BaseURL}transfermarket?start=0&num=21&type=player&definitionId={resourceId}");
            var deserialisedResponse = JsonConvert.DeserializeObject<NetworkAuctionInfo>(response.ResponseString);
            return deserialisedResponse.auctionInfo.Count() < 1 ? 0 : deserialisedResponse.auctionInfo.OrderBy(items => items.BuyNowPrice).First().BuyNowPrice;
        }

        public async Task DeleteSoldItemsFromTradepile()
        {
            Sleep();
            await NetworkTasks.Delete($"{BaseURL}trade/sold");
        }

        public async Task MoveUnassignedItemToTradepileUsingPlayerInternalId(long playerInternalId)
        {
            Sleep();
            var tradepileObject = CreateTradepileObjectUsingPlayerInternalId(playerInternalId);
            await NetworkTasks.Put($"{BaseURL}item", tradepileObject);
        }

        private TradePile CreateTradepileObjectUsingPlayerInternalId(long playerInternalId)
        {
            return new TradePile
            {
                itemData = new List<TradePile.ItemData> {
                    new TradePile.ItemData{
                        id = playerInternalId,
                        pile = "trade"
                    }
                }
            };
        }

        public async Task<NetworkAuctionInfo> GetTradePileAsync()
        {
            Sleep();
            var response = await NetworkTasks.Get($"{BaseURL}tradepile");
            return JsonConvert.DeserializeObject<NetworkAuctionInfo>(response.ResponseString);
        }

        public async Task<bool> BuyItemUsingTradeIdAndBuyNowPriceAsync(long tradeId, long buyNowPrice)
        {
            var response = await NetworkTasks.Put($"{BaseURL}trade/{tradeId}/bid", new Bid { bid = buyNowPrice.ToString() });
            return response.StatusCode == 200;
        }

        public async Task SellItemUsingItemDataIdAndBuyNowPriceAsync(long itemDataId, int buyNowPrice)
        {
            Sleep();
            var sellObject = CreateSellObjectUsingItemIdAndBuyNowPrice(itemDataId, buyNowPrice);
            await NetworkTasks.Post($"{BaseURL}auctionhouse", sellObject);
        }

        private Sell CreateSellObjectUsingItemIdAndBuyNowPrice(long itemId, int buyNowPrice)
        {
            return new Sell
            {
                buyNowPrice = buyNowPrice,
                duration = ViewModel.SelectedDuration,
                startingBid = new General(ViewModel).CalculatePreviousBid(buyNowPrice),
                itemData = new Sell.ItemData { id = itemId }
            };
        }

        public async Task<List<NetworkAuctionInfo.ItemModel>> SearchChemistryStyleUsingChemistryStyleItemAsync(ChemistryStyleItem chemistryStyleItem)
        {
            var pageNumber = 0;
            var allPagesResults = new List<NetworkAuctionInfo.ItemModel>();

            while (true)
            {
                Sleep();
                ChemistryStyleURL = new ChemistryStyleURL(chemistryStyleItem);
                var url = ChemistryStyleURL.GenerateUsingPageNumber(pageNumber++);
                var getResponse = await NetworkTasks.Get(url);
                var deserialisedResponse = JsonConvert.DeserializeObject<NetworkAuctionInfo>(getResponse.ResponseString).auctionInfo;
                allPagesResults.AddRange(deserialisedResponse);
                if (deserialisedResponse.Count() < 21) break;
            }
            return allPagesResults;
        }

        public async Task<List<NetworkAuctionInfo.ItemModel>> SearchPlayerUsingPlayerItemAsync(PlayerItem playerItem)
        {
            var pageNumber = 0;
            var allPagesResults = new List<NetworkAuctionInfo.ItemModel>();

            while (true)
            {
                Sleep();
                PlayerURL = new PlayerURL(playerItem);
                var url = PlayerURL.GenerateUsingPageNumber(pageNumber++);
                var getResponse = await NetworkTasks.Get(url);
                var deserialisedResponse =  JsonConvert.DeserializeObject<NetworkAuctionInfo>(getResponse.ResponseString).auctionInfo;
                allPagesResults.AddRange(deserialisedResponse);
                if (deserialisedResponse.Count() < 21) break;
            }
            return allPagesResults;
        }

        //private string GenerateURL(IMarketplaceItem item)
        //{
        //    switch (item.ItemType)
        //    {
        //        case Declarations.CHEMISTRY_STYLE:
        //            var chemistryStyleURL = new ChemistryStyleURL((ChemistryStyleItem)item);
        //            return chemistryStyleURL.Generate();
        //        case Declarations.PLAYER:
        //            var playerURL = new PlayerURL((PlayerItem)item);
        //            return playerURL.Generate();
        //        default:
        //            throw new InvalidEnumArgumentException();
        //    }
        //}

        private void Sleep()
        {
            Thread.Sleep(new Random().Next(1000, 2000));
        }
    }
}
