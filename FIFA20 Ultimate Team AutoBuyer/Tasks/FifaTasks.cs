using FIFA20_Ultimate_Team_AutoBuyer.Methods;
using FIFA20_Ultimate_Team_AutoBuyer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FIFA20_Ultimate_Team_AutoBuyer.Tasks
{
    public class FifaTasks 
    {
        private readonly viewModel ViewModel;
        private readonly NetworkTasks NetworkTasks;
        private readonly PlayerURL PlayerURL = new PlayerURL();
        private readonly ChemistryStyleURL ChemistryStyleURL = new ChemistryStyleURL();

        public FifaTasks(viewModel viewModel)
        {
            ViewModel = viewModel;
            NetworkTasks = new NetworkTasks(viewModel.SessionID);
        }

        private int CalculateActiveListingsTotalValue(IEnumerable<AuctionInfo.ItemModel> items)
        {
            return items.Where(i => i.Expires > 0).Sum(item => item.BuyNowPrice);
        }

        private int CalculateExpiredListingsTotalValue(IEnumerable<AuctionInfo.ItemModel> items)
        {
            return items.Where(i => i.Expires == -1 && i.CurrentBid == 0).Sum(item => item.BuyNowPrice);
        }

        private async Task<int> CalculateUnlistedListingsTotalValueAsync(IEnumerable<AuctionInfo.ItemModel> items)
        {
            var unlisted = items.Where(i => i.Expires == 0);
            var runningTotal = 0;
            foreach (var item in unlisted)
            {
                Sleep();
                runningTotal += await GetItemSellingPrice(item.ItemData.ItemType, item.ItemData.ResourceId);
            }
            return runningTotal;
        }

        public async Task<int> CalculateAssetsAsync(IEnumerable<AuctionInfo.ItemModel> items)
        {
            if (items == null) return 0;
            var total = 0;
            total += CalculateActiveListingsTotalValue(items);
            total += CalculateExpiredListingsTotalValue(items);
            total += await CalculateUnlistedListingsTotalValueAsync(items);
            return (int)(total * 0.95);
        }

        public async Task<int> GetItemSellingPrice(string type, long definitionID)
        {
            var response = await NetworkTasks.Get($"transfermarket?start=0&num=21&type={type}&definitionId={definitionID}");
            var deserialisedResponse = JsonConvert.DeserializeObject<AuctionInfo>(response.ResponseString);
            return deserialisedResponse.auctionInfo.Count() < 1 ? 0 : deserialisedResponse.auctionInfo.OrderBy(items => items.BuyNowPrice).First().BuyNowPrice;
        }

        public async Task ResolveUnassignedAsync()
        {
            Sleep();
            var response = await NetworkTasks.Get("purchased/items");
            var unassignedItems = JsonConvert.DeserializeObject<Unassigned>(response.ResponseString);
            foreach (var item in unassignedItems.itemdata)
            {
                Thread.Sleep(new Random().Next(4000, 5000));
                await MoveSingleItemToTradePileAsync(item.id);
            }
        }

        public async Task RemoveSoldItemsFromTradepile()
        {
            Sleep();
            await NetworkTasks.Delete("trade/sold");
        }

        public async Task MoveSingleItemToTradePileAsync(long playerInternalID)
        {
            Sleep();
            var tradePileObject = CreateTradepileObject(playerInternalID);
            await NetworkTasks.Put("item", tradePileObject);
        }

        private TradePile CreateTradepileObject(long playerInternalID)
        {
            return new TradePile
            {
                itemData = new List<TradePile.ItemData> {
                    new TradePile.ItemData{
                        id = playerInternalID,
                        pile = "trade"
                    }
                }
            };
        }

        public async Task<AuctionInfo> GetTradePileAsync()
        {
            Sleep();
            var networkTasks = new NetworkTasks(ViewModel.SessionID);
            var response = await networkTasks.Get("{BaseURL}tradepile");
            return JsonConvert.DeserializeObject<AuctionInfo>(response.ResponseString);
        }

        public async Task<bool> BuyItemAsync(long tradeID, long price)
        {
            Sleep();
            var response = await NetworkTasks.Put($"{tradeID}/bid", new Bid { bid = price.ToString() });
            return response.StatusCode == 200;
        }

        public async Task SellItemAsync(long itemID, int buyNowPrice, int duration)
        {
            Sleep();
            var sellObject = CreateSellObject(buyNowPrice, duration, itemID);
            await NetworkTasks.Post("auctionhouse", sellObject);
        }

        private Sell CreateSellObject(int buyNowPrice, int duration, long itemID)
        {
            return new Sell
            {
                buyNowPrice = buyNowPrice,
                duration = duration,
                startingBid = new General(ViewModel).CalculatePreviousBid(buyNowPrice),
                itemData = new Sell.ItemData { id = itemID }
            };
        }

        public async Task<List<AuctionInfo.ItemModel>> SearchMultiplePagesAsync(Filter filter)
        {
            var pageNumber = 0;
            var allResults = new List<AuctionInfo.ItemModel>();

            while (true)
            {
                Sleep();
                var url = PlayerURL.Generate(filter, pageNumber++);
                var getResponse = await NetworkTasks.Get(url);
                var pageResults =  JsonConvert.DeserializeObject<AuctionInfo>(getResponse.ResponseString).auctionInfo;
                allResults.AddRange(pageResults);
                if (pageResults.Count() < 21) break;
            }

            return allResults;
        }

        public async Task<List<AuctionInfo.ItemModel>> SearchAsync(Filter filter)
        {
            Sleep();
            var url = GenerateURL(filter);
            var networkTasks = new NetworkTasks(ViewModel.SessionID);
            var response = await networkTasks.Get(url);
            return JsonConvert.DeserializeObject<AuctionInfo>(response.ResponseString).auctionInfo.ToList();
        }

        private string GenerateURL(Filter filter)
        {
            switch (filter.Type)
            {
                case Declarations.CHEMISTRY_STYLE:
                    return ChemistryStyleURL.Generate(filter);
                case Declarations.PLAYER:
                    return PlayerURL.Generate(filter);
                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        private void Sleep()
        {
            Thread.Sleep(new Random().Next(1000, 2000));
        }
    }
}
