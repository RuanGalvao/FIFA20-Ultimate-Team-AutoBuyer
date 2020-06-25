using FIFA20_Ultimate_Team_AutoBuyer.Methods;
using FIFA20_Ultimate_Team_AutoBuyer.Models;
using FIFA20_Ultimate_Team_AutoBuyer.Tasks;
using System;
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
        private readonly Utils Utils;

        public SearchItemWorker(viewModel viewModel)
        {
            ViewModel = viewModel;
            FifaTasks = new FifaTasks(viewModel);
            General = new General(viewModel);
            Utils = new Utils();
        }

        private int CalculateSearchPrice(IEnumerable<AuctionInfo.ItemModel> results)
        {
            if (results.Count() == 1) return results.OrderBy(item => item.BuyNowPrice).First().BuyNowPrice;
            if (results.Count() == 2) return results.OrderBy(item => item.BuyNowPrice).Take(1).First().BuyNowPrice;
            return results.OrderBy(item => item.BuyNowPrice).Take(2).First().BuyNowPrice;
        }

        private async Task<IEnumerable<AuctionInfo.ItemModel>> GetResults(Filter filter)
        {
            var searchResults = await FifaTasks.SearchMultiplePagesAsync(filter);
            return searchResults?.Where(s => s.ItemData.Rating == filter.Rating);
        }

        private async Task HandleResults(Filter currentFilter, IEnumerable<AuctionInfo.ItemModel> searchResults)
        {
            var filterIndex = ViewModel.SearchFilters.IndexOf(currentFilter);

            if (searchResults == null) return;

            if (currentFilter.SearchPrice == 0 && searchResults.Count() == 0)
            {
                General.AddToLog($"No results found for { currentFilter.GetFriendlyName}");
                return;
            }
            
            if (currentFilter.SearchPrice == 0)
            {
                ViewModel.SearchFilters.ElementAt(filterIndex).SearchPrice = CalculateSearchPrice(searchResults);
                General.AddToLog($"Searching for {currentFilter.GetFriendlyName} at {currentFilter.SearchPrice}");
                return;
            }
            
            if (searchResults.Count() == 0)
            {
                var nextSearchPrice = General.CalculateNextBid(currentFilter.SearchPrice);
                if (nextSearchPrice < currentFilter.MaxPrice)
                {
                    ViewModel.SearchFilters.ElementAt(filterIndex).SearchPrice = nextSearchPrice;
                    General.AddToLog($"Increasing price for {currentFilter.GetFriendlyName} to {currentFilter.SearchPrice}");
                }
                return;
            }
            
            if (searchResults.Count() > 5)
            {
                var nextSearchPrice = General.CalculatePreviousBid(currentFilter.SearchPrice);
                if (nextSearchPrice > currentFilter.MinPrice)
                {
                    ViewModel.SearchFilters.ElementAt(filterIndex).SearchPrice = nextSearchPrice;
                    General.AddToLog($"Decreasing price for {currentFilter.GetFriendlyName} to {currentFilter.SearchPrice}");
                }
                return;
            }

            var broughtItems = new List<long>();
            var maxPrice = General.CalculateMaxBuyNowPrice(currentFilter.SearchPrice, ViewModel.SelectedSellBin) - Utils.ConvertToInt(ViewModel.SelectedMinProfitMargin);
            var items = searchResults.Where(p => currentFilter.Rating == p.ItemData.Rating && p.BuyNowPrice < maxPrice);
            foreach (var item in items)
            {
                if (await FifaTasks.BuyItemAsync(item.TradeId, item.BuyNowPrice))
                {
                    General.AddToLog($"{currentFilter.GetFriendlyName} brought for {item.BuyNowPrice}");

                    var relistPrice = General.CalculateSellPrice(currentFilter.SearchPrice, ViewModel.SelectedSellBin, ViewModel.CurrentCredits);
                    ViewModel.Profit += (int)(relistPrice * .95) - item.BuyNowPrice;

                    broughtItems.Add(item.ItemData.Id);
                }
            }

            // Moves brought items from unassigned to tradepile ready for re-sale
            foreach (var item in broughtItems) await FifaTasks.MoveSingleItemToTradePileAsync(item);
        }

        public async Task Resolve(Filter filter)
        {
            var searchResults = await GetResults(filter);
            await HandleResults(filter, searchResults);
        }
    }
}
