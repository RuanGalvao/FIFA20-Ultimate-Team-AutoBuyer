﻿using System;
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
                .OrderByDescending(x => x.r)
                .Select(x => string.IsNullOrEmpty(x.c) ? $"{x.f} {x.l} {x.r}" : $"{x.c} {x.r}")
                .Where(x => x.IndexOf(SelectedPlayer, 0, StringComparison.InvariantCultureIgnoreCase) != -1 && SelectedPlayer.Length > 0)
                .Take(5)
                .ToList(); 
        }

        public List<Models.Player.PlayerModel> AllPlayers { get => Methods.Player.ReturnAllPlayers(); }
        public ObservableCollection<Models.Log> Log { get; set; } = new ObservableCollection<Models.Log>();
        public ObservableCollection<Models.Search> SearchPlayers { get; set; } = new ObservableCollection<Models.Search>();
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
    }
}
