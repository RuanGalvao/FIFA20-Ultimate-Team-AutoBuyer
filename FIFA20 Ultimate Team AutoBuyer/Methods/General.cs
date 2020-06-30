using FIFA20_Ultimate_Team_AutoBuyer.Models;
using System;
using System.Windows;

namespace FIFA20_Ultimate_Team_AutoBuyer.Methods
{
    public class General
    {
        private readonly viewModel ViewModel;

        public General(viewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public int CalculateMinPrice(int value)
        {
            if (value == 0) return value;
            if (value <= 700) return 700;
            return RoundDownValue(value);
        }

        public int CalculateMaxPrice(int value)
        {
            if (value == 0) return value;
            if (value <= 750) return value;
            return RoundDownValue(value);
        }

        private int RoundDownValue(int value)
        {
            var bid = CalculateNextPriceDifference(value);
            return value -= value % bid;
        }

        public void AddFilter()
        {
            AddSearchFilter();
            ClearSearchFields();
        }

        private void ClearSearchFields()
        {
            ViewModel.SelectedPlayer = "";
            ViewModel.PlayerMaxPrice = "";
            ViewModel.PlayerMinPrice = "";
            ViewModel.PlayerRating = "";
            ViewModel.PlayerMinPrice = "";
            ViewModel.PlayerMaxPrice = "";
            ViewModel.SelectedIndexChemistryStyle = 0;
            ViewModel.SelectedIndexPosition = 0;
            ViewModel.SelectedIndexQuality = 0;
        }

        private void AddPlayerItemToGrid()
        {
            ViewModel.MarketplaceItems.Add(new PlayerItem
            {
                Id = Player.GetID(ViewModel.SelectedPlayer.Substring(0, ViewModel.SelectedPlayer.Length - 3), CalculatePlayerRating()),
                FriendlyName = ViewModel.SelectedPlayer.Substring(0, ViewModel.SelectedPlayer.Length - 3),
                Position = ViewModel.SelectedPosition,
                Quality = CalculateCardQuality(ViewModel.SelectedType),
                ChemistryStyle = ViewModel.SelectedChemistryStyle,
                Rating = CalculatePlayerRating(),
                Sell = ViewModel.SelectedSellItem,
                MinPrice = ViewModel.SelectedMinPrice,
                MaxPrice = ViewModel.SelectedMaxPrice
            });
        }

        private void AddChemistryStyleItemToGrid()
        {
            ViewModel.MarketplaceItems.Add(new ChemistryStyleItem
            {
                Id = ChemistryStyle.GetID(ViewModel.SelectedChemistryStyle),
                Quality = CalculateCardQuality(ViewModel.SelectedType),
                FriendlyName = ViewModel.SelectedChemistryStyle,
                Rating = CalculateChemistryStyleRating(),
                Sell = ViewModel.SelectedSellItem
            });
        }

        private void AddSearchFilter()
        {
            if (ViewModel.SelectedType == Declarations.PLAYER) AddPlayerItemToGrid();
            if (ViewModel.SelectedType == Declarations.CHEMISTRY_STYLE) AddChemistryStyleItemToGrid();
        }

        private string CalculatePlayerQuality()
        {
            if (Convert.ToInt32(ViewModel.SelectedRating) > 0) return "Special";
            if (ViewModel.SelectedOriginalRating < 65) return "Bronze";
            if (ViewModel.SelectedOriginalRating < 75) return "Silver";
            return "Gold";
        }

        public string CalculateCardQuality(string type)
        {
            if (type == Declarations.CHEMISTRY_STYLE) return "Gold";
            if (type == Declarations.PLAYER) return CalculatePlayerQuality();
            throw new InvalidOperationException();
        }

        private int CalculateChemistryStyleRating()
        {
            return 95;
        }

        private int CalculatePlayerRating()
        {
            return ViewModel.SelectedQuality == "Special" ? ViewModel.SelectedRating : ViewModel.SelectedOriginalRating;
        }

        public void AddToLog(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ViewModel.Log.Insert(0, new Log
                {
                    Time = DateTime.Now.ToLongTimeString(),
                    Message = message
                });
            });
        }

        public int CalculateProfitUsingSellPriceAndBroughtPrice(int sellPrice, int broughtPrice)
        {
            return (int)(sellPrice * .95) - broughtPrice;
        }

        public int AmendBidBasedOnSelectedSellBin(int currentPrice)
        {
            switch (ViewModel.SelectedSellBin)
            {
                case "Automatic":
                    // Determine sell price dependant on the amount of credits avail
                    if (currentPrice * 5 > ViewModel.CurrentCredits) return CalculatePreviousBid(currentPrice);
                    if (currentPrice * 15 < ViewModel.CurrentCredits) return CalculateNextBid(currentPrice);
                    return currentPrice;
                case "Very Low":
                    var veryLowPrice = CalculatePreviousBid(currentPrice);
                    return CalculatePreviousBid(veryLowPrice);
                case "Low":
                    return CalculatePreviousBid(currentPrice);
                case "Medium":
                    return currentPrice;
                case "High":
                    return CalculateNextBid(currentPrice);
            }
            return currentPrice;
        }

        public int CalculateMaxBuyNowPriceUsingSearchPrice(int searchPrice)
        {
            var searchPriceMinusTax = (int)(searchPrice * 0.95);
            return AmendBidBasedOnSelectedSellBin(searchPriceMinusTax);
        }

        public int CalculateNextPriceDifference(int currentBid)
        {
            if (currentBid < 1000) return 50;
            if (currentBid < 10000) return 100;
            if (currentBid < 50000) return 250;
            if (currentBid < 100000) return 500;
            return 1000;
        }

        public int CalculateNextBid(int currentBid)
        {
            if (currentBid <= 200) return 200;
            if (currentBid < 1000) return currentBid + 50;
            if (currentBid < 10000) return currentBid + 100;
            if (currentBid < 50000) return currentBid + 250;
            if (currentBid < 100000) return currentBid + 500;
            return currentBid + 1000;
        }

        public int CalculatePreviousBid(int currentBid)
        {
            if (currentBid <= 200) return 200;
            if (currentBid <= 1000) return currentBid - 50;
            if (currentBid <= 10000) return currentBid - 100;
            if (currentBid <= 50000) return currentBid - 250;
            if (currentBid <= 100000) return currentBid - 500;
            return currentBid - 1000;
        }
    }
}
