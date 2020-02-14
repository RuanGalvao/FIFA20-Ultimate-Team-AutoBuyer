using FIFA20_Ultimate_Team_Autobuyer.Models;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FIFA20_Ultimate_Team_Autobuyer.Methods
{
    public class Search
    {
        internal async Task<string> Player(InternalPlayer currentPlayer, string sessionID)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-UT-SID", sessionID);
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("user-agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.90 Safari/537.36");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                var generatedSearchURL = new Search().GeneratePlayerSearchURL(currentPlayer);

                var response = await httpClient.GetAsync(generatedSearchURL);

                if (!response.IsSuccessStatusCode) throw new Exception(Convert.ToInt32(response.StatusCode).ToString());

                return await response.Content.ReadAsStringAsync();
            }
        }

        internal async Task<int> GetPlayerSellingPriceAsync(InternalPlayer currentPlayer, string sessionID)
        {
            Thread.Sleep(5000);

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-UT-SID", sessionID);
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("user-agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.90 Safari/537.36");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                var generatedSearchURL = new Search().GeneratePlayerSearchURL(currentPlayer);
                var response = await httpClient.GetAsync(generatedSearchURL);

                if (response.StatusCode != HttpStatusCode.OK) throw new HttpRequestException(Convert.ToInt32(response.StatusCode).ToString());

                var responseString = await response.Content.ReadAsStringAsync();

                var playerData = JsonConvert.DeserializeObject<AuctionInfo>(responseString);

                return playerData.auctionInfo.Where(item => item.ItemData.Rating == currentPlayer.Rating).OrderBy(r => r.BuyNowPrice).First().BuyNowPrice;
            }
        }

        protected string GeneratePlayerSearchURL(InternalPlayer currentPlayer)
        {
            var url = $"https://utas.external.s3.fut.ea.com/ut/game/fifa20/transfermarket?start=0&num=21&type=player&maskedDefId={currentPlayer.ID}";
            var bidPriceMin = new Random().Next(3, 13) * 50;
            var bidPriceMax = bidPriceMin + 50;

            if (currentPlayer.IsSpecial) url += $"&rare=SP";

            url += $"&micr={bidPriceMin}";

            if (currentPlayer.SearchPrice > 0) return url += $"&macr={CalculateBid.CalculatePreviousBid(currentPlayer.SearchPrice)}&minb={bidPriceMax}&maxb={currentPlayer.SearchPrice}";
            if (currentPlayer.MaxPrice > 0) return url += $"&macr={CalculateBid.CalculatePreviousBid(currentPlayer.MaxPrice)}&minb={currentPlayer.MinPrice}&maxb={currentPlayer.MaxPrice}";

            return url;
        }
    }
}
