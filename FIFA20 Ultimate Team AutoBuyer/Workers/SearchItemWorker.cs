using FIFA20_Ultimate_Team_AutoBuyer.Methods;
using FIFA20_Ultimate_Team_AutoBuyer.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FIFA20_Ultimate_Team_AutoBuyer.Workers
{
    public class SearchItemWorker
    {
        private readonly FifaTasks FifaTasks;
        private readonly viewModel ViewModel;
        private readonly General General;

        public SearchItemWorker(viewModel viewModel)
        {
            ViewModel = viewModel;
            FifaTasks = new FifaTasks(viewModel);
            General = new General(viewModel);
        }

        private int CalculateSearchPriceUsingTradePileData(IEnumerable<NetworkAuctionInfo.ItemModel> tradepile)
        {
            if (tradepile.Count() == 1) return tradepile.OrderBy(item => item.BuyNowPrice).First().BuyNowPrice;
            if (tradepile.Count() == 2) return tradepile.OrderBy(item => item.BuyNowPrice).Take(1).First().BuyNowPrice;
            return tradepile.OrderBy(item => item.BuyNowPrice).ElementAt(2).BuyNowPrice;
        }

        private async Task<IEnumerable<NetworkAuctionInfo.ItemModel>> SearchResultsUsingMarketplaceItemFilteredByRatingAsync(IMarketplaceItem item)
        {
            List<NetworkAuctionInfo.ItemModel> searchResults = null;
            if (item.ItemType == Declarations.PLAYER) 
                searchResults = await FifaTasks.SearchPlayerUsingPlayerItemAsync((PlayerItem)item);

            if (item.ItemType == Declarations.CHEMISTRY_STYLE)
                searchResults = await FifaTasks.SearchChemistryStyleUsingChemistryStyleItemAsync((ChemistryStyleItem)item);

            return searchResults?.Where(s => s.ItemData.Rating == item.Rating);
        }

        private bool IsSearchPriceEqualToMaxPrice(int searchPrice, int maxPrice)
        {
            return searchPrice == maxPrice && maxPrice != 0;
        }

        private bool IsSearchPriceEqualToMinPrice(int searchPrice, int minPrice)
        {
            return searchPrice == minPrice && minPrice != 0;
        }

        private async Task HandleResults(IMarketplaceItem marketplaceItem, IEnumerable<NetworkAuctionInfo.ItemModel> searchResults)
        {
            var filterIndex = ViewModel.MarketplaceItems.IndexOf(marketplaceItem);

            if (searchResults == null) return;

            if (marketplaceItem.SearchPrice == 0 && searchResults.Count() == 0)
            {
                General.AddToLog($"No results found for { marketplaceItem.FriendlyName}");
                return;
            }
            
            if (marketplaceItem.SearchPrice == 0)
            {
                ViewModel.MarketplaceItems.ElementAt(filterIndex).SearchPrice = CalculateSearchPriceUsingTradePileData(searchResults);
                General.AddToLog($"Searching for {marketplaceItem.FriendlyName} at {marketplaceItem.SearchPrice}");
                return;
            }
            
            if (searchResults.Count() == 0)
            {
                if (!IsSearchPriceEqualToMaxPrice(marketplaceItem.SearchPrice, marketplaceItem.MaxPrice))
                {
                    //Raise search price
                    var nextSearchPrice = General.CalculateNextBid(marketplaceItem.SearchPrice);
                    ViewModel.MarketplaceItems.ElementAt(filterIndex).SearchPrice = nextSearchPrice;
                    General.AddToLog($"Increasing price for {marketplaceItem.FriendlyName} to {marketplaceItem.SearchPrice}");
                }
                return;
            }
            
            if (searchResults.Count() > 5)
            {
                if (!IsSearchPriceEqualToMinPrice(marketplaceItem.SearchPrice, marketplaceItem.MinPrice))
                {
                    //Reduce search price
                    var nextSearchPrice = General.CalculatePreviousBid(marketplaceItem.SearchPrice);
                    ViewModel.MarketplaceItems.ElementAt(filterIndex).SearchPrice = nextSearchPrice;
                    General.AddToLog($"Decreasing price for {marketplaceItem.FriendlyName} to {marketplaceItem.SearchPrice}");
                }
                return;
            }

            var broughtItems = new List<long>();
            var maxPrice = General.CalculateMaxBuyNowPriceUsingSearchPrice(marketplaceItem.SearchPrice) - ViewModel.SelectedMinProfitMargin;
            foreach (var item in searchResults.Where(p => p.BuyNowPrice < maxPrice))
            {
                if (await FifaTasks.BuyItemUsingTradeIdAndBuyNowPriceAsync(item.TradeId, item.BuyNowPrice))
                {
                    General.AddToLog($"{marketplaceItem.FriendlyName} brought for {item.BuyNowPrice}");
                    var relistPrice = General.AmendBidBasedOnSelectedSellBin(marketplaceItem.SearchPrice);
                    ViewModel.Profit += General.CalculateProfitUsingSellPriceAndBroughtPrice(relistPrice, item.BuyNowPrice);
                    broughtItems.Add(item.ItemData.Id);
                }
            }

            // Moves brought items from unassigned to tradepile ready for re-sale
            foreach (var item in broughtItems) await FifaTasks.MoveUnassignedItemToTradepileUsingPlayerInternalId(item);
        }

        public async Task DoWork(IMarketplaceItem item)
        {
            var searchResults = await SearchResultsUsingMarketplaceItemFilteredByRatingAsync(item);
            await HandleResults(item, searchResults);
        }
    }
}
