using FIFA20_Ultimate_Team_Autobuyer.Enums;
using FIFA20_Ultimate_Team_Autobuyer.Methods;
using FIFA20_Ultimate_Team_Autobuyer.Models;
using FIFA20_Ultimate_Team_Autobuyer.General;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MessageBox = System.Windows.Forms.MessageBox;

namespace FIFA20_Ultimate_Team_Autobuyer
{
    public partial class MainWindow : Window
    {
        public ViewModel ViewModel => (ViewModel)DataContext;

        private Utils utils = new Utils();

        private DateTime nextRunTime;
        private TimeSpan addDelay;

        private readonly string APPLICATION_NAME = "FIFA20 Ultimate Team AutoBuyer";

        private readonly string CHEMISTRY_STYLE = "Chemistry Style";
        private readonly string PLAYER = "Player";

        private bool isConnected = false;
        private bool justConnected = false;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModel();

            Title = APPLICATION_NAME;

            nextRunTime = DateTime.Now;
            addDelay = new TimeSpan(0, 0, 0);

            var startingCredits = 0;
            var currentCredits = 0;
            var filterIndex = 0;
            var errorCount = 0;
            var sessionID = "";
            var sellPriceBin = "";
            var enableSelling = true;
            var url = "";

            var search = new Search();

            var filters = new List<Filter>();

            var tradePile = new Methods.TradePile();

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
                        if (!isConnected) foreach (var item in ViewModel.SearchFilters) item.SearchPrice = 0;
                        filters = ViewModel.SearchFilters.ToList();
                        enableSelling = ViewModel.EnableSelling;
                    });

                    if (isConnected && DateTime.Now > nextRunTime)
                    {
                        try
                        {
                            var currentFilter = filters.ElementAt(filterIndex % filters.Count);
                            filterIndex++;

                            // User has just clicked start. Move unassigned items to tradepile ready for sale
                            if (justConnected)
                            {
                                justConnected = false;

                                await tradePile.ResolveUnassignedAsync(sessionID);

                                var tradePileData = await tradePile.GetAsync(sessionID);

                                startingCredits = tradePileData.credits;
                                currentCredits = tradePileData.credits;

                                var assetsTotalValue = await tradePile.CalculateAssetsAsync(tradePileData?.auctionInfo, filters, sessionID);

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
                                    if (ViewModel.StartingCredits == 0) ViewModel.StartingCredits = startingCredits + assetsTotalValue;
                                    ViewModel.CurrentCredits = startingCredits;
                                    ViewModel.Total = startingCredits + assetsTotalValue;
                                });
                            }

                            // If is the current filter is new get his selling price
                            if (currentFilter.SearchPrice == 0)
                            {
                                url = search.GenerateURL(currentFilter);
                                var data = await search.ItemAsync(url, sessionID);
                                var playerData = JsonConvert.DeserializeObject<AuctionInfo>(data);
                                currentFilter.SearchPrice = playerData.auctionInfo
                                    .Where(item => item.ItemData.Rating == currentFilter.Rating)
                                    .OrderBy(r => r.BuyNowPrice)
                                    .First().BuyNowPrice;
                                UpdateCurrentFilter("SearchPrice", currentFilter.SearchPrice.ToString(), (filterIndex - 1) % filters.Count);
                                AddToLog($"Searching for {currentFilter.GetFriendlyName} at {currentFilter.SearchPrice}");
                                Sleep();
                            }

                            url = search.GenerateURL(currentFilter);
                            var searchData = await search.ItemAsync(url, sessionID);

                            var filterResponse = JsonConvert.DeserializeObject<AuctionInfo>(searchData);
                            if (filterResponse == null) continue;

                            if (filterResponse.auctionInfo.Count() == 0)
                            {
                                // Unable to find listings. Increase price if less than max price
                                var nextSearchPrice = CalculateBid.CalculateNextBid(currentFilter.SearchPrice);
                                if (nextSearchPrice < currentFilter.MaxPrice)
                                {
                                    UpdateCurrentFilter("SearchPrice", CalculateBid.CalculateNextBid(currentFilter.SearchPrice).ToString(), (filterIndex - 1) % filters.Count);
                                    AddToLog($"Increasing price for {currentFilter.GetFriendlyName} to {currentFilter.SearchPrice}");
                                }
                            }
                            else if (filterResponse.auctionInfo.Count() > 10)
                            {
                                // Too many listings found. Reduce price if greater than min price
                                var nextSearchPrice = CalculateBid.CalculatePreviousBid(currentFilter.SearchPrice);
                                if (nextSearchPrice > currentFilter.MinPrice)
                                {
                                    UpdateCurrentFilter("SearchPrice", CalculateBid.CalculatePreviousBid(currentFilter.SearchPrice).ToString(), (filterIndex - 1) % filters.Count);
                                    AddToLog($"Decreasing price for {currentFilter.GetFriendlyName} to {currentFilter.SearchPrice}");
                                }
                            }
                            else
                            {
                                // We have some listings to work with
                                var broughtItems = new List<long>();
                                var sell = new Methods.Sell();
                                var buy = new Buy();

                                var items = filterResponse.auctionInfo.Where(p => currentFilter.Rating == p.ItemData.Rating && p.BuyNowPrice < currentFilter.SearchPrice * 0.95 - CalculateBid.CalculateMinProfitMargin(currentFilter.SearchPrice));
                                foreach (var item in items)
                                {
                                    if (await buy.ItemAsync(item.TradeId, item.BuyNowPrice, sessionID))
                                    {
                                        AddToLog($"{currentFilter.GetFriendlyName} brought for {item.BuyNowPrice}");

                                        var relistPrice = sell.CalculatePrice(currentFilter.SearchPrice, sellPriceBin, currentCredits);
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

                                // Checks the status of the Tradepile updating the UI. Only do this when an item has been brought
                                // to reduce the number of calls made
                                if (broughtItems.Count() > 0)
                                {
                                    Sleep();
                                    var tradePileData = await tradePile.GetAsync(sessionID);
                                    currentCredits = tradePileData.credits;

                                    // Update UI
                                    var assetsTotal = await tradePile.CalculateAssetsAsync(tradePileData.auctionInfo, filters, sessionID);
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
                                        if (item.ItemData.ItemType.ToLower() == "player")
                                        {
                                            AddToLog($"{Player.GetName(item.ItemData.AssetId)} {Player.GetRating(item.ItemData.AssetId)} sold for {item.CurrentBid}");

                                        }
                                        else
                                        {
                                            AddToLog($"{ChemistryStyle.GetName(item.ItemData.CardSubTypeId)} sold for {item.CurrentBid}");
                                                                                    
                                        }
                                        Sleep();
                                        await tradePile.DeleteAsync(item.TradeId, sessionID);
                                    }

                                    // Items that are not listed for sale
                                    var availableItems = tradePileData?.auctionInfo.Where(s => s.Expires <= 0 && (s.ItemData.CardSubTypeId == currentFilter.ID || s.ItemData.AssetId == currentFilter.ID)  && currentFilter.Sell);
                                    foreach (var item in availableItems)
                                    {
                                        var relistPrice = sell.CalculatePrice(currentFilter.SearchPrice, sellPriceBin, currentCredits);
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

                        nextRunTime = DateTime.Now + new TimeSpan(0, 0, 5) + addDelay;
                        addDelay = new TimeSpan(0, 0, 0);

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
                case FIFAUltimateTeamStatusCode.InsufficentFunds:
                    AddToLog("Insufficient Funds - Sleeping for 5 mins");
                    addDelay = new TimeSpan(0, 5, 0);
                    break;
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
                    if (ex.Message.Contains("Unable to find player"))
                    {
                        Dispatcher.Invoke(() => { MessageBox.Show("Unable to find Player. Please check filters and re-try", APPLICATION_NAME); });
                        isConnected = false;
                        errorCount = 0;
                        break;
                    }

                    if (errorCount > 5)
                    {
                        Dispatcher.Invoke(() => { MessageBox.Show("Unknown Error Searching stopped", APPLICATION_NAME); });
                        isConnected = false;
                        errorCount = 0;
                    }
                    break;
            }
        }

        private void UpdateCurrentFilter(string property, string value, int index)
        {
            Dispatcher.Invoke(() =>
            {
                switch (property.ToUpper())
                { 
                    case "ID":
                        ViewModel.SearchFilters.ElementAt(index).ID = utils.ConvertToInt(value);
                        break;
                    case "SEARCHPRICE":
                        ViewModel.SearchFilters.ElementAt(index).SearchPrice = utils.ConvertToInt(value);
                        break;
                    case "NAME":
                        ViewModel.SearchFilters.ElementAt(index).PlayerName = value;
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
                MessageBox.Show("Please enter a Session ID", APPLICATION_NAME);
                return;
            }

            // Check if player list contains any elements
            if (ViewModel.SearchFilters.Count == 0)
            {
                MessageBox.Show("Please add a Filter to the list", APPLICATION_NAME);
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
            if (ViewModel.SelectedType == PLAYER)
            {
                if (!AllowPlayerFilter()) return;

                // Validation complete. Player can be added
                ViewModel.SearchFilters.Add(new Filter
                {
                    Type = PLAYER,
                    ID = Player.GetID(
                        ViewModel.SelectedPlayer.Substring(0, ViewModel.SelectedPlayer.Length - 3).Trim(),
                        utils.ConvertToInt(ViewModel.SelectedPlayer.Substring(ViewModel.SelectedPlayer.Length - 2, 2))),
                    PlayerName = ViewModel.SelectedPlayer.Substring(0, ViewModel.SelectedPlayer.Length - 3),
                    Position = string.IsNullOrEmpty(ViewModel.SelectedPosition) ? "" : ViewModel.SelectedPosition,
                    Quality = ViewModel.SelectedQuality,
                    ChemistryStyle = ViewModel.SelectedChemistryStyle,
                    Rating = !string.IsNullOrEmpty(ViewModel.PlayerRating) ? utils.ConvertToInt(ViewModel.PlayerRating) : utils.ConvertToInt(ViewModel.SelectedPlayer.Substring(ViewModel.SelectedPlayer.Length - 2, 2)),
                    MinPrice = !string.IsNullOrEmpty(ViewModel.PlayerRating) ? utils.ConvertToInt(ViewModel.PlayerMinPrice) : 0,
                    MaxPrice = !string.IsNullOrEmpty(ViewModel.PlayerRating) ? utils.ConvertToInt(ViewModel.PlayerMaxPrice) : 0,
                    Sell = ViewModel.SelectedSellItem
                });
            }
            else
            {
                if (!AllowChemistryFilter()) return;

                ViewModel.SearchFilters.Add(new Filter
                {
                    Type = CHEMISTRY_STYLE,
                    ID = ChemistryStyle.GetID(ViewModel.SelectedChemistryStyle),
                    PlayerName = "",
                    Position = "",
                    Quality = "",
                    ChemistryStyle = ViewModel.SelectedChemistryStyle,
                    Rating = 95,
                    MinPrice = utils.ConvertToInt(ViewModel.PlayerMinPrice),
                    MaxPrice = utils.ConvertToInt(ViewModel.PlayerMaxPrice),
                    Sell = ViewModel.SelectedSellItem
                });
            }
            
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

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            // Do not allow the list to be empty while searching
            if (ViewModel.SearchFilters.Count == 1 && ViewModel.IsConnected)
            {
                MessageBox.Show("Player list cannot be empty while searching", APPLICATION_NAME);
                return;
            }

            // Validation complete. Filter can be removed from the list
            ViewModel.SearchFilters.Remove((Models.Filter)DataGridPlayers1.SelectedItem);
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            txtSessionID.Focus();
        }

        private void cboPlayer_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            cboPlayer.IsDropDownOpen = true;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private bool AllowPlayerFilter()
        {
            if (string.IsNullOrEmpty(ViewModel.SelectedPlayer))
            {
                MessageBox.Show($"Please select a Player from the drop-down", APPLICATION_NAME);
                return false;
            }

            if (ViewModel.SearchFilters.Any(p => p.GetFriendlyName == ViewModel.SelectedPlayer.Substring(0, ViewModel.SelectedPlayer.Length - 2) + ViewModel.PlayerRating))
            {
                MessageBox.Show($"{ViewModel.SelectedPlayer} has already been added", APPLICATION_NAME);
                ViewModel.SelectedPlayer = "";
                return false;
            }

            // Check if the item exists in the dropdown
            if (!ViewModel.Players.Any(list => list.Contains(ViewModel.SelectedPlayer)))
            {
                MessageBox.Show("Invalid player entered", APPLICATION_NAME);
                ViewModel.SelectedPlayer = "";
                return false;
            }

            var playerInternalID = 0;
            playerInternalID = Player.GetID(
            ViewModel.SelectedPlayer.Substring(0, ViewModel.SelectedPlayer.Length - 3).Trim(),
            utils.ConvertToInt(ViewModel.SelectedPlayer.Substring(ViewModel.SelectedPlayer.Length - 2, 2)));
            
            if (playerInternalID == 0)
            {
                MessageBox.Show("Unable to retrieve internal player ID", APPLICATION_NAME);
                ViewModel.SelectedPlayer = "";
                return false;
            }

            var minPrice = ViewModel.PlayerMinPrice == "" ? 0 : utils.ConvertToInt(ViewModel.PlayerMinPrice);
            var maxPrice = ViewModel.PlayerMaxPrice == "" ? 0 : utils.ConvertToInt(ViewModel.PlayerMaxPrice);
            var rating = ViewModel.PlayerRating == "" ? 0 : utils.ConvertToInt(ViewModel.PlayerRating);

            if (rating > 99 || rating < 0)
            {
                MessageBox.Show("Player rating must be between 1-99", APPLICATION_NAME);
                ViewModel.PlayerRating = "";
                return false;
            }

            if ((minPrice == 0 || maxPrice == 0) && ViewModel.SelectedQuality == "Special")
            {
                MessageBox.Show("Please specify min and max price", APPLICATION_NAME);
                return false;
            }

            if (minPrice > 9999999 || maxPrice > 9999999)
            {
                MessageBox.Show("Player price range must be between 0-9,999,999", APPLICATION_NAME);
                ViewModel.PlayerMinPrice = "";
                ViewModel.PlayerMaxPrice = "";
                return false;
            }

            if (minPrice > maxPrice && maxPrice != 0)
            {
                MessageBox.Show("Player min price cannot exceed max price", APPLICATION_NAME);
                ViewModel.PlayerMinPrice = "";
                ViewModel.PlayerMaxPrice = "";
                return false;
            }

            return true;
        }

        private bool AllowChemistryFilter()
        {
            if (string.IsNullOrEmpty(ViewModel.SelectedChemistryStyle))
            {
                MessageBox.Show($"Please select a Chemistry Style from the drop-down", APPLICATION_NAME);
                return false;
            }

            if (ViewModel.SearchFilters.Any(p => p.PlayerName == ViewModel.SelectedChemistryStyle))
            {
                MessageBox.Show($"{ViewModel.SelectedChemistryStyle} has already been added", APPLICATION_NAME);
                ViewModel.SelectedPlayer = "";
                return false;
            }

            var internalID = 0;
            internalID = ChemistryStyle.GetID(ViewModel.SelectedChemistryStyle);

            if (internalID == 0)
            {
                MessageBox.Show("Unable to retrieve internal Chemistry Style ID", APPLICATION_NAME);
                ViewModel.SelectedPlayer = "";
                return false;
            }

            var minPrice = ViewModel.PlayerMinPrice == "" ? 0 : utils.ConvertToInt(ViewModel.PlayerMinPrice);
            var maxPrice = ViewModel.PlayerMaxPrice == "" ? 0 : utils.ConvertToInt(ViewModel.PlayerMaxPrice);
            var rating = ViewModel.PlayerRating == "" ? 0 : utils.ConvertToInt(ViewModel.PlayerRating);

            if (rating > 99 || rating < 0)
            {
                MessageBox.Show("Player rating must be between 1-99", APPLICATION_NAME);
                ViewModel.PlayerRating = "";
                return false;
            }

            if (minPrice > 9999999 || maxPrice > 9999999)
            {
                MessageBox.Show("Player price range must be between 0-9,999,999", APPLICATION_NAME);
                ViewModel.PlayerMinPrice = "";
                ViewModel.PlayerMaxPrice = "";
                return false;
            }

            if (minPrice > maxPrice && maxPrice != 0)
            {
                MessageBox.Show("Player min price cannot exceed max price", APPLICATION_NAME);
                ViewModel.PlayerMinPrice = "";
                ViewModel.PlayerMaxPrice = "";
                return false;
            }

            return true;
        }

        private void txtMinPrice_LostFocus(object sender, RoutedEventArgs e)
        {
            var value = utils.ConvertToInt(txtMinPrice.Text);
            value -= value % CalculateBid.CalculateMinProfitMargin(value);
            if (value < 700) txtMinPrice.Text = "700";
        }

        private void txtMaxPrice_LostFocus(object sender, RoutedEventArgs e)
        {
            var value = utils.ConvertToInt(txtMaxPrice.Text);
            value -= value % CalculateBid.CalculateMinProfitMargin(value);
            if (value < 750) txtMaxPrice.Text = "750";
        }

        private void txtRating_LostFocus(object sender, RoutedEventArgs e)
        {
            var value = utils.ConvertToInt(txtMinPrice.Text);
            if (value > 99) txtRating.Text = "99";
        }
    }
}