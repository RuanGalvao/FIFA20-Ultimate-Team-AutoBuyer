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
        internal async Task<string> Player(int playerID, int searchPrice, string sessionID)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-UT-SID", sessionID);
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("user-agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.90 Safari/537.36");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                var generatedSearchURL = new Search().GeneratePlayerSearchURL(searchPrice, playerID);

                var response = await httpClient.GetAsync(generatedSearchURL);

                if (!response.IsSuccessStatusCode) throw new Exception(Convert.ToInt32(response.StatusCode).ToString());

                return await response.Content.ReadAsStringAsync();
            }
        }

        internal async Task<int> GetPlayerSellingPriceAsync(int playerID, string sessionID)
        {
            Thread.Sleep(5000);

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-UT-SID", sessionID);
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("user-agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.90 Safari/537.36");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                var generatedSearchURL = new Search().GeneratePlayerSearchURL(0, playerID);
                var response = await httpClient.GetAsync(generatedSearchURL);

                if (response.StatusCode != HttpStatusCode.OK) throw new HttpRequestException(Convert.ToInt32(response.StatusCode).ToString());

                var responseString = await response.Content.ReadAsStringAsync();

                var playerData = JsonConvert.DeserializeObject<AuctionInfo>(responseString);

                return playerData.auctionInfo.OrderBy(r => r.BuyNowPrice).First().BuyNowPrice;
            }
        }

        protected string GeneratePlayerSearchURL(int startPrice, int playerID)
        {
            var baseSearchURL = $"https://utas.external.s3.fut.ea.com/ut/game/fifa20/transfermarket?start=0&num=21&type=player";
            var buyNowMinPrice = new Random().Next(6, 13) * 50;
            var bidPriceMinPrice = new Random().Next(3, (buyNowMinPrice - 50) / 50) * 50;

            if (startPrice == 0) return $"{baseSearchURL}&maskedDefId={playerID}&micr={bidPriceMinPrice}&minb={buyNowMinPrice}";

            return $"{baseSearchURL}&maskedDefId={playerID}&micr={bidPriceMinPrice}&minb={buyNowMinPrice}&maxb={startPrice}";
        }
    }
}
