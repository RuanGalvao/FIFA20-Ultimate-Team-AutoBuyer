using FIFA20_Ultimate_Team_AutoBuyer.Methods;
using FIFA20_Ultimate_Team_AutoBuyer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FIFA20_Ultimate_Team_AutoBuyer
{
    public class viewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public List<string> Players { get => 
                AllPlayers
                .OrderByDescending(x => x.Rating)
                .Select(player => $"{player.FriendlyName} {player.Rating}")
                .Where(x => x.IndexOf(SelectedPlayer, 0, StringComparison.InvariantCultureIgnoreCase) != -1 && SelectedPlayer.Length > 0)
                .Take(5)
                .ToList(); 
        }

        public List<string> Positions { get => new List<string> { "", "Defenders", "Midfielders", "Attackers", "GK", "RWB", "RB", "CB", "LB",
            "LWB", "CDM", "RM", "CM", "LM", "CAM", "RF", "CF", "LF", "RW", "ST", "LW"}; }

        public List<string> Types { get => new List<string> { "Player", "Chemistry Style"}; }

        public IEnumerable<string> ChemistryStyles => new List<string> { "" }.Concat(ChemistryStyle.ReturnAllChemistrystyles().ToList());

        public List<string> Qualities { get => new List<string> { "", "Bronze", "Silver", "Gold", "Special"}; }

        public List<string> SellItem { get => new List<string> { "True", "False" }; }

        public List<PlayerItem> AllPlayers { get => Player.GetAll(); }
        public ObservableCollection<Log> Log { get; set; } = new ObservableCollection<Log>();
        public ObservableCollection<IMarketplaceItem> MarketplaceItems { get; set; } = new ObservableCollection<IMarketplaceItem>();

        public ObservableCollection<TradePileGrid> Tradepile { get; set; } = new ObservableCollection<TradePileGrid>();

        public List<string> SellPriceBin { get => new List<string> { "Very Low", "Low", "Medium", "High", "Automatic" }; }
        public string SelectedSellBin { get; set; } = "Very Low";

        public List<Duration> Durations
        {
            get => new List<Duration> {
                new Duration {Name = "1 Hour", Seconds = 3600 },
                new Duration {Name = "3 Hours", Seconds = 10800 },
                new Duration {Name = "6 Hours", Seconds = 21600 },
                new Duration {Name = "12 Hours", Seconds = 43200 },
                new Duration {Name = "1 Day", Seconds = 86400},
                new Duration {Name = "3 Days", Seconds = 259200 }
            };
        }

        public string Duration { get; set; } = "3 Hours";

        public int SelectedDuration => Durations.Where(p => p.Name == Duration).First().Seconds;

        public string SelectedType { get; set; } = "Player";
        public string SelectedPlayer { get; set; } = "";
        public string SelectedChemistryStyle { get; set; } = "";

        public int SelectedChemistryStyleId => string.IsNullOrEmpty(SelectedChemistryStyle) ? 0 : ChemistryStyle.GetID(SelectedChemistryStyle);

        public string SelectedQuality { get; set; }
        public string SelectedPosition { get; set; }
        public bool SelectedSellItem { get; set; }

        public List<string> MinProfitMarginList { get => new List<string> { "1000", "2000", "3000", "4000", "5000" }; }
        public string MinProfitMargin { get; set; } = "4000";
        public int SelectedMinProfitMargin => Convert.ToInt32(MinProfitMargin);

        public string SessionID { get; set; }
        public int StartingCredits { get; set; } = 0;
        public int CurrentCredits { get; set; } = 0;
        public int Assets { get; set; } = 0;
        public int Total { get; set; } = 0;
        public int Profit { get; set; } = 0;
        public bool IsConnected { get; set; }
        public string ConnectButton { get => IsConnected ? "Stop" : "Start"; }
        public string StartButtonColour { get => !IsConnected && StartingCredits != 0 ? "OrangeRed" : "ForestGreen" ; }
        public bool EnableFields { get => !IsConnected; }

        public bool DisableChemistryStyleFields => SelectedType == "Player";

        public string PlayerMinPrice { get; set; }
        public string PlayerMaxPrice { get; set; }



        public int SelectedMinPrice => string.IsNullOrEmpty(PlayerMinPrice) ? 0 : Convert.ToInt32(PlayerMinPrice);
        public int SelectedMaxPrice => string.IsNullOrEmpty(PlayerMaxPrice) ? 0 : Convert.ToInt32(PlayerMaxPrice);

        public int SelectedRating => string.IsNullOrEmpty(PlayerRating) ? 0 : Convert.ToInt32(PlayerRating);

        public int SelectedIndexChemistryStyle { get; set; }
        public int SelectedIndexPosition { get; set; }
        public int SelectedIndexQuality { get; set; }
        public string PlayerRating { get; set; }
        public bool EnableSelling { get; set; } = true;

        public int SelectedOriginalRating => Convert.ToInt32(SelectedPlayer.Substring(SelectedPlayer.Length - 2, 2));

        public bool DisableRating => SelectedType == "Player" && SelectedQuality == "Special";
        public bool DisableMinPrice => SelectedType == "Player" && SelectedQuality == "Special";
        public bool DisableMaxPrice => SelectedType == "Player" && SelectedQuality == "Special";

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach(var item in MarketplaceItems)
            {
                if (item.ItemType == Declarations.PLAYER)
                {
                    var playerItem = (PlayerItem)item;
                    sb.Append(Declarations.PLAYER + ",");
                    sb.Append(playerItem.Id.ToString() + ",");
                    sb.Append(playerItem.Position + ",");
                    sb.Append(playerItem.Quality + ",");
                    sb.Append(playerItem.ChemistryStyle + ",");
                    sb.Append(playerItem.Rating + ",");
                    sb.Append(playerItem.MinPrice + ",");
                    sb.Append(playerItem.MaxPrice + ",");
                    sb.Append(playerItem.Sell);
                    sb.Append("\n");
                }
                if (item.ItemType == Declarations.CHEMISTRY_STYLE)
                {
                    var chemistryStyleItem = (ChemistryStyleItem)item;
                    sb.Append(Declarations.CHEMISTRY_STYLE + ',');
                    sb.Append(chemistryStyleItem.Id.ToString() + ',');
                    sb.Append(",");
                    sb.Append(chemistryStyleItem.Quality + ",");
                    sb.Append(",");
                    sb.Append(chemistryStyleItem.Rating + ",");
                    sb.Append(chemistryStyleItem.MinPrice + ",");
                    sb.Append(chemistryStyleItem.MaxPrice + ",");
                    sb.Append(chemistryStyleItem.Sell);
                    sb.Append("\n");
                } 
            }

            var toString = sb.ToString();
            return toString.Length == 0 ? "" : toString.Substring(0, toString.Length - 1);
        }
    }
}
