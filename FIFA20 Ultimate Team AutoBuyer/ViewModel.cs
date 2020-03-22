using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace FIFA20_Ultimate_Team_Autobuyer
{
    public class ViewModel : INotifyPropertyChanged
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
        public ObservableCollection<Models.Log> Log { get; set; } = new ObservableCollection<Models.Log>();
        public ObservableCollection<Models.Filter> SearchFilters { get; set; } = new ObservableCollection<Models.Filter>();
        public List<string> SellPriceBin { get => new List<string> { "Low", "Medium", "High", "Automatic" }; }
        public string SelectedType { get; set; } = "Player";
        public string SelectedPlayer { get; set; } = "";
        public string SelectedChemistryStyle { get; set; } = "";
        public string SelectedQuality { get; set; }
        public string SelectedPosition { get; set; }
        public bool SelectedSellItem { get; set; }

        public string SelectedSellPrice { get; set; } = "Low";
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

        public string PlayerRating { get; set; }

        public bool EnableSelling { get; set; } = true;
    }
}
