using FIFA20_Ultimate_Team_Autobuyer.Enums;
using FIFA20_Ultimate_Team_Autobuyer.Methods;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using MessageBox = System.Windows.Forms.MessageBox;

namespace FIFA20_Ultimate_Team_Autobuyer
{
    public partial class MainWindow : Window
    {
        public ViewModel ViewModel => (ViewModel)DataContext;

        private DateTime nextRunTime;

        private readonly string APPLICATION_NAME = "FIFA20 Ultimate Team AutoBuyer";

        private bool isConnected = false;
        private bool justConnected = false;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModel();

            nextRunTime = DateTime.Now;

            var startingCredits = 0;
            var currentCredits = 0;
            var playerIndex = 0;
            var errorCount = 0;
            var sessionID = "";
            var sellPriceBin = "";

            var players = new List<Models.InternalPlayer>();

            var tradePile = new TradePile();

            var x = Task.Run(async () =>
            {
                while (true)
                {
                    isConnected = false;
                    
                    Dispatcher.Invoke(() =>
                    {
                        isConnected = ViewModel.IsConnected;
                        sessionID = ViewModel.SessionID;
                        sellPriceBin = ViewModel.SelectedSellPrice;
                        if (!isConnected) foreach (var item in ViewModel.SearchPlayers) item.SearchPrice = 0;
                        players = ViewModel.SearchPlayers.ToList();
                    });

                    if (isConnected && DateTime.Now > nextRunTime)
                    {
                        try
                        {
                            // User has just clicked start. Move unassigned items to tradepile ready for sale
                            if (justConnected)
                            {
                                justConnected = false;

                                await tradePile.ResolveUnassignedAsync(sessionID);

                                var tradePileData = await tradePile.GetAsync(sessionID);

                                startingCredits = tradePileData.credits;
                                currentCredits = tradePileData.credits;

                                var assetsTotalValue = await tradePile.CalculateAssetsAsync(tradePileData?.auctionInfo, sessionID);

                                // Clear sold items before we begin searching
                                var soldItems = tradePileData?.auctionInfo.Where(s => s.Expires == -1 && s.CurrentBid != 0);
                                if (soldItems.Count() > 0) AddToLog("Removing sold items");
                                foreach (var item in soldItems)
                                {
                                    Sleep();
                                    await tradePile.DeleteAsync(item.TradeId, sessionID);
                                }

                                Dispatcher.Invoke(() =>
                                {
                                    ViewModel.IsConnected = true;
                                    ViewModel.Assets = assetsTotalValue;
                                    ViewModel.StartingCredits = startingCredits;
                                    ViewModel.CurrentCredits = startingCredits;
                                    ViewModel.Total = startingCredits + assetsTotalValue;
                                });
                            }

                            var currentPlayer = players.ElementAt(playerIndex % players.Count);
                            playerIndex++;

                            // This player has just been added to the list. Get his selling price
                            if (currentPlayer.SearchPrice == 0)
                            {
                                currentPlayer.SearchPrice = await new Search().GetPlayerSellingPriceAsync(currentPlayer.ID, sessionID);
                                currentPlayer.SearchPrice = CalculateBid.CalculatePreviousBid(currentPlayer.SearchPrice);
                                UpdatePlayerObject("SearchPrice", currentPlayer.SearchPrice.ToString(), (playerIndex - 1) % players.Count);
                                AddToLog($"Searching for {currentPlayer.NameRating} at {currentPlayer.SearchPrice}");
                                Sleep();
                            }

                            var search = new Search();
                            var searchData = await search.Player(currentPlayer.ID, currentPlayer.SearchPrice, sessionID);

                            var playerData = JsonConvert.DeserializeObject<AuctionInfo>(searchData);

                            if (playerData == null) continue;

                            if (playerData.auctionInfo.Count() == 0)
                            {
                                // Unable to find listings. Increase price
                                UpdatePlayerObject("SearchPrice", CalculateBid.CalculateNextBid(currentPlayer.SearchPrice).ToString(), (playerIndex - 1) % players.Count);
                                AddToLog($"Increasing price for {currentPlayer.NameRating} to {currentPlayer.SearchPrice}");
                            }
                            else if (playerData.auctionInfo.Count() > 10)
                            {
                                // Too many listings found. Reduce price
                                UpdatePlayerObject("SearchPrice", CalculateBid.CalculatePreviousBid(currentPlayer.SearchPrice).ToString(), (playerIndex - 1) % players.Count);
                                AddToLog($"Decreasing price for {currentPlayer.NameRating} to {currentPlayer.SearchPrice}");
                            }
                            else
                            {
                                // We have some listings to work with
                                var items = playerData.auctionInfo.Where(p => p.BuyNowPrice < currentPlayer.SearchPrice * 0.95 - CalculateBid.CalculateMinProfitMargin(currentPlayer.SearchPrice));
                                
                                var broughtItems = new List<long>();
                                var sell = new Sell();
                                foreach (var item in items)
                                {
                                    var buy = new Buy();
                                    if (await buy.PlayerAsync(item.TradeId, item.BuyNowPrice, sessionID))
                                    {
                                        AddToLog($"{currentPlayer.NameRating} brought for {item.BuyNowPrice}");

                                        var relistPrice = sell.CalculatePrice(currentPlayer.SearchPrice, sellPriceBin, currentCredits);
                                        Dispatcher.Invoke(() =>
                                        {
                                            ViewModel.Profit += (int)(relistPrice * .95) - item.BuyNowPrice;
                                        });

                                        Sleep();
                                        broughtItems.Add(item.ItemData.Id);
                                    }
                                }

                                // Moves brought items from unassigned to tradepile ready for re-sale
                                foreach (var item in broughtItems)
                                {
                                    Sleep();
                                    await tradePile.MoveToTradePileAsync(item, sessionID);
                                }

                                // Checks the status of the Tradepile undating the UI when necessary. Only do this when an item has been brought
                                // to reduce the number of calls made
                                if (broughtItems.Count() > 0)
                                {
                                    Sleep();
                                    var tradePileData = await tradePile.GetAsync(sessionID);
                                    currentCredits = tradePileData.credits;

                                    // Update UI
                                    var assetsTotal = await tradePile.CalculateAssetsAsync(tradePileData.auctionInfo, sessionID);
                                    Dispatcher.Invoke(() =>
                                     {
                                         ViewModel.CurrentCredits = tradePileData.credits;
                                         ViewModel.Assets = assetsTotal;
                                         ViewModel.Total = (tradePileData.credits + assetsTotal);
                                     });

                                    // Check to see if any items have sold
                                    var soldItems = tradePileData?.auctionInfo.Where(s => s.Expires == -1 && s.CurrentBid != 0);
                                    foreach (var item in soldItems)
                                    {
                                        AddToLog($"{Player.GetPlayerName(item.ItemData.AssetId)} {Player.GetPlayerRating(item.ItemData.AssetId)} sold for {item.CurrentBid}");
                                        Sleep();
                                        await tradePile.DeleteAsync(item.TradeId, sessionID);
                                    }

                                    //Items that are not listed for sale
                                    var availableItems = tradePileData?.auctionInfo.Where(s => s.Expires <= 0 && s.ItemData.AssetId == currentPlayer.ID);
                                    foreach (var item in availableItems)
                                    {
                                        var relistPrice = sell.CalculatePrice(currentPlayer.SearchPrice, sellPriceBin, currentCredits);
                                        await sell.ItemAsync(item.ItemData.Id, relistPrice, sessionID);
                                        Sleep();
                                    }
                                }
                            }
                        } 
                        catch (Exception ex)
                        { 
                            HandleError(ex, ref errorCount);
                        }
                        nextRunTime = DateTime.Now + new TimeSpan(0, 0, 5);

                        if (!isConnected)
                        {
                            Dispatcher.Invoke(() => { ViewModel.IsConnected = false; });
                        }
                    }
                    Thread.Sleep(100);
                }
            });
        }

        private void HandleError(Exception ex, ref int errorCount)
        {
            Enum.TryParse(ex.Message, out FIFAUltimateTeamStatusCode value);

            switch (value)
            {
                case FIFAUltimateTeamStatusCode.CaptureRequired:
                    MessageBox.Show("Please complete the CAPTCHA on the Web App and re-enter the Session ID", APPLICATION_NAME);
                    Dispatcher.Invoke(() => { ViewModel.SessionID = ""; });
                    isConnected = false;
                    break;
                case FIFAUltimateTeamStatusCode.Unauthorized:
                    errorCount++;
                    if (errorCount > 5)
                    {
                        MessageBox.Show("Invalid Session ID. Please re-enter a Session ID", APPLICATION_NAME);
                        Dispatcher.Invoke(() => { ViewModel.SessionID = ""; });
                        isConnected = false;
                        errorCount = 0;
                    }
                    break;
                case FIFAUltimateTeamStatusCode.Sold:
                    break;
                default:
                    if (ex.Message.Contains("One or more errors occurred.")) break; // This is probably due to a time-out
                    errorCount++;
                    if (errorCount > 5)
                    {
                        Dispatcher.Invoke(() => { MessageBox.Show("Unknown Error. Searching stopped.", APPLICATION_NAME); });
                        isConnected = false;
                        errorCount = 0;
                    }
                    break;
            }
        }

        private void UpdatePlayerObject(string property, string value, int index)
        {
            Dispatcher.Invoke(() =>
            {
                switch (property.ToUpper())
                { 
                    case "ID":
                        ViewModel.SearchPlayers.ElementAt(index).ID = Convert.ToInt32(value);
                        break;
                    case "SEARCHPRICE":
                        ViewModel.SearchPlayers.ElementAt(index).SearchPrice = Convert.ToInt32(value);
                        break;
                    case "NAME":
                        ViewModel.SearchPlayers.ElementAt(index).Name = value;
                        break;
                }
            });
        }

        private void AddToLog(string message)
        {
            Dispatcher.Invoke(() =>
            {
                ViewModel.Log.Insert(0, new Models.Log { 
                    Time = DateTime.Now.ToLongTimeString(),
                    Message = message 
                });
            });
        }

        private void Sleep()
        {
            Thread.Sleep(new Random().Next(4000, 5000));
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            // Check if SessionID has been entered
            if (string.IsNullOrEmpty((ViewModel.SessionID)))
            {
                MessageBox.Show("Invalid Session ID", APPLICATION_NAME);
                return;
            }

            // Check if player list contains any elements
            if (ViewModel.SearchPlayers.Count == 0)
            {
                MessageBox.Show("Please add a player to the list", APPLICATION_NAME);
                return;
            }

            if (!ViewModel.IsConnected)
            {
                nextRunTime = DateTime.Now;
                justConnected = true;
            }

            ViewModel.IsConnected = !ViewModel.IsConnected;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            // Check if dropdown text is empty
            if (string.IsNullOrEmpty(ViewModel.SelectedPlayer))
            {
                MessageBox.Show("Please select a player from the drop-down", APPLICATION_NAME);
                return;
            }

            // Check if the player has already been added to the list
            if (ViewModel.SearchPlayers.Any(p => p.Name == ViewModel.SelectedPlayer))
            {
                MessageBox.Show($"{ViewModel.SelectedPlayer} has already been added", APPLICATION_NAME);
                ViewModel.SelectedPlayer = "";
                return;
            }

            // Check if the player exists in the dropdown
            if (!ViewModel.Players.Any(list => list.Contains(ViewModel.SelectedPlayer)))
            {
                MessageBox.Show("Invalid player entered", APPLICATION_NAME);
                ViewModel.SelectedPlayer = "";
                return;
            }

            var playerInternalID = 0;
            playerInternalID = Player.GetPlayerID(ViewModel.SelectedPlayer.Substring(0, ViewModel.SelectedPlayer.Length - 3).Trim());

            if (playerInternalID == 0)
            {
                MessageBox.Show("Unable to retrieve internal player ID", APPLICATION_NAME);
                ViewModel.SelectedPlayer = "";
                return;
            }

            // Validation complete. Player can be added
            ViewModel.SearchPlayers.Add(new Models.InternalPlayer
            {
                ID = playerInternalID,
                Name = ViewModel.SelectedPlayer.Substring(0, ViewModel.SelectedPlayer.Length - 3),
                Rating = Convert.ToInt32(ViewModel.SelectedPlayer.Substring(ViewModel.SelectedPlayer.Length - 2, 2))
            });

            ViewModel.SelectedPlayer = "";
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            // Do not allow the list to be empty while searching
            if (ViewModel.SearchPlayers.Count == 1 && ViewModel.IsConnected)
            {
                MessageBox.Show("Player list cannot be empty while searching", APPLICATION_NAME);
                return;
            }

            // Validation complete. Player can be removed from the list
            ViewModel.SearchPlayers.Remove((Models.InternalPlayer)listViewPlayers.SelectedItem);
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            txtSessionID.Focus();
        }

        private void cboPlayer_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            cboPlayer.IsDropDownOpen = true;
        }
    }
}