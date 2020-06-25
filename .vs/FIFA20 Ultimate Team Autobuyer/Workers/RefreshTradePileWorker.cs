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

        private async Task UpdateUIStats(AuctionInfo tradePile)
        {
            var assetsTotalValue = await FifaTasks.CalculateAssetsAsync(tradePile.auctionInfo);
            ViewModel.CurrentCredits = tradePile.credits;
            if (ViewModel.StartingCredits == 0) ViewModel.StartingCredits = tradePile.credits + assetsTotalValue;
            ViewModel.CurrentCredits = tradePile.credits;
            ViewModel.Assets = assetsTotalValue;
            ViewModel.Total = tradePile.credits + assetsTotalValue;
        }

        private void ReportItemSold(AuctionInfo.ItemModel soldItem)
        {
            if (soldItem.ItemData.ItemType == Declarations.PLAYER.ToLower())
                General.AddToLog($"{Player.GetName(soldItem.ItemData.AssetId)} {Player.GetRating(soldItem.ItemData.AssetId)} sold for {soldItem.CurrentBid}");

            if (soldItem.ItemData.ItemType == Declarations.PLAYER.ToLower())
                General.AddToLog($"{ChemistryStyle.GetName(soldItem.ItemData.CardSubTypeId)} sold for {soldItem.CurrentBid}");
        }

        private async Task ResolveSoldItems(IEnumerable<AuctionInfo.ItemModel> soldItems)
        {
            foreach (var item in soldItems) ReportItemSold(item);
            if (soldItems.Count() > 0) await FifaTasks.RemoveSoldItemsFromTradepile();
        }

        private async Task RelistItem(Filter filter, AuctionInfo.ItemModel item)
        {
            var relistPrice = General.CalculateSellPrice(filter.SearchPrice, ViewModel.SelectedSellBin, ViewModel.CurrentCredits);
            await FifaTasks.SellItemAsync(item.ItemData.Id, relistPrice, ViewModel.Durations.Where(p => p.Name == ViewModel.SelectedDuration).First().Seconds);
        }

        private async Task ResolveUnlistedItems(IEnumerable<AuctionInfo.ItemModel> unlistedItems)
        {
            foreach (var filter in ViewModel.SearchFilters)
            {
                var currentFilterUnlistedItems = unlistedItems
                    .Where(unlistedItem => (filter.ID == unlistedItem.ItemData.CardSubTypeId || filter.ID == unlistedItem.ItemData.AssetId) && filter.Sell && filter.SearchPrice > 0);
                foreach (var tradepileItem in currentFilterUnlistedItems) await RelistItem(filter, tradepileItem);
            }
        }

        public async Task DoWork()
        {
            var tradePileData = await FifaTasks.GetTradePileAsync();
            await UpdateUIStats(tradePileData);
            await ResolveSoldItems(tradePileData?.auctionInfo.Where(s => s.Expires == -1 && s.CurrentBid != 0));
            await ResolveUnlistedItems(tradePileData?.auctionInfo.Where(s => s.Expires <= 0));
            PopulateTradePileGrid(tradePileData);
        }

        public void PopulateTradePileGrid(AuctionInfo tradepileData)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ViewModel.TradePile = new ObservableCollection<TradePileGrid>();
                foreach (var item in tradepileData.auctionInfo) ViewModel.TradePile.Add(CreateTradePileGridObject(item));
            });
        }

        private TradePileGrid CreateTradePileGridObject(AuctionInfo.ItemModel item)
        {
            return new TradePileGrid
            {
                Name = item.ItemData.ItemType == "player" ? Player.GetName(item.ItemData.AssetId) : ChemistryStyle.GetName(item.ItemData.CardSubTypeId),
                Duration = item.Expires,
                Status = item.TradeState,
                Rating = item.ItemData.Rating,
                Type = item.ItemData.ItemType == "player" ? Declarations.PLAYER : Declarations.CHEMISTRY_STYLE,
                BuyNowPrice = item.BuyNowPrice
            };
        }
    }
}
