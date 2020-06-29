using FIFA20_Ultimate_Team_AutoBuyer.Methods;
using FIFA20_Ultimate_Team_AutoBuyer.Models;
using FIFA20_Ultimate_Team_AutoBuyer.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FIFA20_Ultimate_Team_AutoBuyer.Workers
{
    public class RefreshTradePileWorker
    {
        private readonly viewModel ViewModel;
        private readonly FifaTasks FifaTasks;
        private readonly General General;

        public RefreshTradePileWorker(viewModel viewModel)
        {
            ViewModel = viewModel;
            FifaTasks = new FifaTasks(viewModel);
            General = new General(viewModel);
        }

        private async Task UpdateUIStatsUsingTradepileData(NetworkAuctionInfo tradepile)
        {
            var assetsTotalValue = await FifaTasks.CalculateTradepileAssetsTotalValue(tradepile.auctionInfo);
            ViewModel.CurrentCredits = tradepile.credits;
            if (ViewModel.StartingCredits == 0) ViewModel.StartingCredits = tradepile.credits + assetsTotalValue;
            ViewModel.CurrentCredits = tradepile.credits;
            ViewModel.Assets = assetsTotalValue;
            ViewModel.Total = tradepile.credits + assetsTotalValue;
        }

        private void ReportItemSoldToLog(NetworkAuctionInfo.ItemModel soldItem)
        {
            if (soldItem.ItemData.ItemType == Declarations.PLAYER.ToLower())
                General.AddToLog($"{Player.GetName(soldItem.ItemData.AssetId)} {Player.GetRating(soldItem.ItemData.AssetId)} sold for {soldItem.CurrentBid}");

            if (soldItem.ItemData.ItemType == "training") // Chemistry Style
                General.AddToLog($"{ChemistryStyle.GetName(soldItem.ItemData.CardSubTypeId)} sold for {soldItem.CurrentBid}");
        }

        private async Task RemoveSoldItems(IEnumerable<NetworkAuctionInfo.ItemModel> soldItems)
        {
            foreach (var item in soldItems) ReportItemSoldToLog(item);
            if (soldItems.Count() > 0) await FifaTasks.DeleteSoldItemsFromTradepile();
        }

        private async Task RelistItem(IMarketplaceItem marketplaceItem, NetworkAuctionInfo.ItemModel item)
        {
            var relistPrice = General.AmendBidBasedOnSelectedSellBin(marketplaceItem.SearchPrice);
            await FifaTasks.SellItemUsingItemDataIdAndBuyNowPriceAsync(item.ItemData.Id, relistPrice);
        }

        private async Task RelistUnsoldItems(IEnumerable<NetworkAuctionInfo.ItemModel> unlistedItems)
        {
            foreach (var filter in ViewModel.MarketplaceItems)
            {
                var currentFilterUnlistedItems = unlistedItems
                    .Where(unlistedItem => (filter.Id == unlistedItem.ItemData.CardSubTypeId || filter.Id == unlistedItem.ItemData.AssetId) && filter.Sell && filter.SearchPrice > 0);
                foreach (var tradepileItem in currentFilterUnlistedItems) await RelistItem(filter, tradepileItem);
            }
        }

        public async Task DoWork()
        {
            var tradepileData = await FifaTasks.GetTradePileAsync();
            await UpdateUIStatsUsingTradepileData(tradepileData);
            await RemoveSoldItems(tradepileData?.auctionInfo.Where(s => s.Expires == -1 && s.CurrentBid != 0));
            await RelistUnsoldItems(tradepileData?.auctionInfo.Where(s => s.Expires <= 0));
            PopulateTradePileGrid(tradepileData);
        }

        public void PopulateTradePileGrid(NetworkAuctionInfo tradepileData)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ViewModel.Tradepile = new ObservableCollection<TradePileGrid>();
                foreach (var item in tradepileData.auctionInfo) AddItemToTradepileGrid(item);
            });
        }

        private void AddItemToTradepileGrid(NetworkAuctionInfo.ItemModel item)
        {
            ViewModel.Tradepile.Add(new TradePileGrid
            {
                Name = item.ItemData.ItemType == "player" ? Player.GetName(item.ItemData.AssetId) : ChemistryStyle.GetName(item.ItemData.CardSubTypeId),
                Duration = item.Expires,
                Status = item.TradeState,
                Rating = item.ItemData.Rating,
                Type = item.ItemData.ItemType == "player" ? Declarations.PLAYER : Declarations.CHEMISTRY_STYLE,
                BuyNowPrice = item.BuyNowPrice
            });
        }
    }
}
