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
                .Select(player => $"{player.Name} {player.Rating}")
                .Where(x => x.IndexOf(SelectedPlayer, 0, StringComparison.InvariantCultureIgnoreCase) != -1 && SelectedPlayer.Length > 0)
                .Take(5)
                .ToList(); 
        }

        public List<Models.InternalPlayer> AllPlayers { get => Methods.Player.ReturnAllPlayers(); }
        public ObservableCollection<Models.Log> Log { get; set; } = new ObservableCollection<Models.Log>();
        public ObservableCollection<Models.InternalPlayer> SearchPlayers { get; set; } = new ObservableCollection<Models.InternalPlayer>();
        public List<string> SellPriceBin { get => new List<string> { "Low", "Medium", "High", "Automatic" }; }
        public string SelectedPlayer { get; set; } = "";
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


        public string PlayerMinPrice { get; set; }
        public string PlayerMaxPrice { get; set; }

        public string PlayerRating { get; set; }
        public bool PlayerIsSpecial { get; set; } = false;
        public bool EnableAdvanced { get => PlayerIsSpecial; }

        public bool EnableSelling { get; set; } = true;
    }
}
