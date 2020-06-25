using FIFA20_Ultimate_Team_AutoBuyer.Models;
using System;
using System.Windows;

namespace FIFA20_Ultimate_Team_AutoBuyer.Methods
{
    public class General
    {
        private readonly viewModel ViewModel;
        private readonly Utils Utils = new Utils();

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
            ViewModel.SelectedRating = "";
            ViewModel.PlayerMinPrice = "";
            ViewModel.PlayerMaxPrice = "";
            ViewModel.SelectedIndexChemistryStyle = 0;
            ViewModel.SelectedIndexPosition = 0;
            ViewModel.SelectedIndexQuality = 0;
        }

        private int GetItemID()
        {
            if (ViewModel.SelectedType == Declarations.PLAYER) return Player.GetID(ViewModel.SelectedPlayer.Substring(0, ViewModel.SelectedPlayer.Length - 3));
            if (ViewModel.SelectedType == Declarations.CHEMISTRY_STYLE) return ChemistryStyle.GetID(ViewModel.SelectedChemistryStyle);
            throw new InvalidOperationException();
        }

        private string GetPlayerName()
        {
            if (ViewModel.SelectedType == Declarations.PLAYER) return ViewModel.SelectedPlayer.Substring(0, ViewModel.SelectedPlayer.Length - 3);
            if (ViewModel.SelectedType == Declarations.CHEMISTRY_STYLE) return "";
            throw new InvalidOperationException();
        }

        private void AddSearchFilter()
        {
            ViewModel.SearchFilters.Add(new Filter
            {
                Type = ViewModel.SelectedType,
                ID = GetItemID(),
                PlayerName = GetPlayerName(),
                Position = ViewModel.SelectedPosition,
                Quality = CalculateCardQuality(ViewModel.SelectedType),
                ChemistryStyle = ViewModel.SelectedChemistryStyle,
                Rating = CalculateRating(),
                MinPrice = Utils.ConvertToInt(ViewModel.PlayerMinPrice),
                MaxPrice = Utils.ConvertToInt(ViewModel.PlayerMaxPrice),
                Sell = ViewModel.SelectedSellItem
            });
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

        private int CalculatePlayerRating()
        {
            if (ViewModel.SelectedQuality == "Special") return Utils.ConvertToInt(ViewModel.SelectedRating);
            return ViewModel.SelectedOriginalRating;
        }

        private int CalculateRating()
        {
            if (ViewModel.SelectedType == Declarations.CHEMISTRY_STYLE) return 95;
            if (ViewModel.SelectedType == Declarations.PLAYER) return CalculatePlayerRating();
            throw new InvalidOperationException();
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

        public int CalculateSellPrice(int searchPrice, string option, int currentCredits)
        {
            switch (option.ToUpper())
            {
                case "AUTOMATIC":
                    // Determine sell price dependant on the amount of credits avail
                    if (searchPrice * 5 > currentCredits) return CalculatePreviousBid(searchPrice);
                    if (searchPrice * 15 < currentCredits) return CalculateNextBid(searchPrice);
                    return searchPrice;
                case "VERY LOW":
                    var veryLowPrice = CalculatePreviousBid(searchPrice);
                    return CalculatePreviousBid(veryLowPrice);
                case "LOW":
                    return CalculatePreviousBid(searchPrice);
                case "MEDIUM":
                    return searchPrice;
                case "HIGH":
                    return CalculateNextBid(searchPrice);
            }
            return searchPrice;
        }

        public int CalculateMaxBuyNowPrice(int currentBid, string sellPriceBin)
        {
            var returnPrice = (int)(currentBid * 0.95);
            if (sellPriceBin.ToUpper() == "VERY LOW") returnPrice -= CalculateNextPriceDifference(returnPrice);
            return returnPrice - CalculateNextPriceDifference(returnPrice);
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
