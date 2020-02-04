using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace FIFA20_Ultimate_Team_Autobuyer.Methods
{
    public class Sell
    {
        internal async Task ItemAsync(long itemID, int buyNowPrice, string sessionID)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-UT-SID", sessionID);
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("user-agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.90 Safari/537.36");

                var listObject = new Models.Sell
                {
                    buyNowPrice = buyNowPrice,
                    duration = 3600,
                    startingBid = CalculateBid.CalculatePreviousBid(buyNowPrice),
                    itemData = new Models.Sell.ItemData
                    {
                        id = itemID
                    }
                };

                var listObjectSerialised = JsonConvert.SerializeObject(listObject);

                var response = await httpClient.PostAsync($"https://utas.external.s3.fut.ea.com/ut/game/fifa20/auctionhouse", new StringContent(listObjectSerialised));
            }
        }

        internal int CalculatePrice(int searchPrice, string option, int currentCredits)
        {
            switch (option.ToUpper())
            {
                case "AUTOMATIC":
                    // Determine sell price dependant on the amount of credits avail
                    if (searchPrice * 5 > currentCredits) return CalculateBid.CalculatePreviousBid(searchPrice);
                    if (searchPrice * 15 < currentCredits) return CalculateBid.CalculateNextBid(searchPrice);
                    return searchPrice;
                case "LOW":
                    return CalculateBid.CalculatePreviousBid(searchPrice);
                case "MEDIUM":
                    return searchPrice;
                case "HIGH":
                    return CalculateBid.CalculateNextBid(searchPrice);
            }

            return searchPrice;
        }
    }
}
