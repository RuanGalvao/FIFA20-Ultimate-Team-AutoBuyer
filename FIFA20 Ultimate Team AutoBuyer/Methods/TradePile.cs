using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using FIFA20_Ultimate_Team_Autobuyer.Models;

namespace FIFA20_Ultimate_Team_Autobuyer.Methods
{
    public class TradePile
    {
        internal async Task<int> CalculateAssetsAsync(IEnumerable<AuctionInfo.ItemModel> items, List<InternalPlayer> players, string sessionID)
        {
            if (items == null) return 0;

            var total = 0;

            //Currently for sale
            total += items.Where(i => i.Expires > 0).Sum(item => item.BuyNowPrice);

            //Expired listings
            total += items.Where(i => i.Expires == -1 && i.CurrentBid == 0).Sum(item => item.BuyNowPrice);

            //Unlisted items
            total += (await Task.WhenAll(
                items.Where(i => i.Expires == 0 && players.Any(a => a.ID == i.ItemData.AssetId))
                .Select(i => new Search().GetPlayerSellingPriceAsync(players.Where(x => x.ID == i.ItemData.AssetId).Select(x => x).FirstOrDefault(), sessionID))
                .ToArray()))
                .Sum();

            return (int) (total * 0.95);
        }

        internal async Task<AuctionInfo> GetAsync(string sessionID)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-UT-SID", sessionID);
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("user-agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.90 Safari/537.36");
                var response = await httpClient.GetAsync("https://utas.external.s3.fut.ea.com/ut/game/fifa20/tradepile");

                var stringResponse = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<AuctionInfo>(stringResponse);
            }
        }

        public async Task ResolveUnassignedAsync(string sessionID)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-UT-SID", sessionID);
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("user-agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.90 Safari/537.36");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                var response = await httpClient.GetAsync("https://utas.external.s3.fut.ea.com/ut/game/fifa20/purchased/items");

                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode) throw new Exception(Convert.ToInt32(response.StatusCode).ToString());

                var unassignedItems = JsonConvert.DeserializeObject<Unassigned>(responseString);

                foreach (var item in unassignedItems.itemdata)
                {
                    Thread.Sleep(new Random().Next(4000, 5000));
                    await MoveToTradePileAsync(item.id, sessionID);
                }
            }
        }

        internal async Task MoveToTradePileAsync(long playerInternalID, string sessionID)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-UT-SID", sessionID);
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("user-agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.90 Safari/537.36");

                var tradePileObject = new Models.TradePile
                {
                    itemData = new List<Models.TradePile.ItemData> {
                        new Models.TradePile.ItemData{
                            id = playerInternalID, 
                            pile = "trade"
                        }
                    }
                };

                var tradePileObjectSerialised = JsonConvert.SerializeObject(tradePileObject);

                var response = await httpClient.PutAsync("https://utas.external.s3.fut.ea.com/ut/game/fifa20/item", new StringContent(tradePileObjectSerialised));
            }
        }

        internal async Task DeleteAsync(long tradeID, string sessionID)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-UT-SID", sessionID);
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("user-agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.90 Safari/537.36");
                var response = await httpClient.DeleteAsync($"https://utas.external.s3.fut.ea.com/ut/game/fifa20/trade/{tradeID}");
            }
        }
    }
}
