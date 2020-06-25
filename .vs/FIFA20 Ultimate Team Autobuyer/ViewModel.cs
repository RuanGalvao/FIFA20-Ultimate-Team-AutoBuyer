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
                .Select(player => $"{player.PlayerName} {player.Rating}")
                .Where(x => x.IndexOf(SelectedPlayer, 0, StringComparison.InvariantCultureIgnoreCase) != -1 && SelectedPlayer.Length > 0)
                .Take(5)
                .ToList(); 
        }

        public List<string> Positions { get => new List<string> { "", "Defenders", "Midfielders", "Attackers", "GK", "RWB", "RB", "CB", "LB",
            "LWB", "CDM", "RM", "CM", "LM", "CAM", "RF", "CF", "LF", "RW", "ST", "LW"}; }

        public List<string> Types { get => new List<string> { "Player", "Chemistry Style"}; }

        public IEnumerable<string> ChemistryStyles => new List<string> { "" }.Concat(Methods.ChemistryStyle.ReturnAllChemistrystyles().ToList());

        public List<string> Qualities { get => new List<string> { "", "Bronze", "Silver", "Gold", "Special"}; }

        public List<string> SellItem { get => new List<string> { "True", "False" }; }

        public List<Models.Filter> AllPlayers { get => Methods.Player.GetAll(); }
        public ObservableCollection<Log> Log { get; set; } = new ObservableCollection<Log>();
        public ObservableCollection<Filter> SearchFilters { get; set; } = new ObservableCollection<Filter>();

        public ObservableCollection<TradePileGrid> TradePile { get; set; } = new ObservableCollection<TradePileGrid>();

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var s in SearchFilters)
            {
                sb.Append(s.Type + ",");
                sb.Append(s.ID.ToString() + ",");
                sb.Append(s.Position + ",");
                sb.Append(s.Quality + ",");
                sb.Append(s.ChemistryStyle + ",");
                sb.Append(s.Rating + ",");
                sb.Append(s.MinPrice + ",");
                sb.Append(s.MaxPrice + ",");
                sb.Append(s.Sell);
                sb.Append("\n");
            }
            return sb.ToString();
        }

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

        public string SelectedDuration { get; set; } = "3 Hours";

        public string SelectedType { get; set; } = "Player";
        public string SelectedPlayer { get; set; } = "";
        public string SelectedChemistryStyle { get; set; } = "";
        public string SelectedQuality { get; set; }
        public string SelectedPosition { get; set; }
        public bool SelectedSellItem { get; set; }

        public List<string> MinProfitMargin { get => new List<string> { "1000", "2000", "3000", "4000", "5000" }; }
        public string SelectedMinProfitMargin { get; set; } = "4000";


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

        public int SelectedIndexChemistryStyle { get; set; }
        public int SelectedIndexPosition { get; set; }
        public int SelectedIndexQuality { get; set; }
        public string SelectedRating { get; set; }
        public bool EnableSelling { get; set; } = true;

        public int SelectedOriginalRating => Convert.ToInt32(SelectedPlayer.Substring(SelectedPlayer.Length - 2, 2));

        public bool DisableRating => SelectedType == "Player" && SelectedQuality == "Special";
        public bool DisableMinPrice => SelectedType == "Player" && SelectedQuality == "Special";
        public bool DisableMaxPrice => SelectedType == "Player" && SelectedQuality == "Special";
    }
}
