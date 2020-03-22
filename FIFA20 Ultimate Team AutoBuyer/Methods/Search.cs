using FIFA20_Ultimate_Team_Autobuyer.Models;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FIFA20_Ultimate_Team_Autobuyer.Methods
{
    public class Search
    {
        internal async Task<string> ItemAsync(string url, string sessionID)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-UT-SID", sessionID);
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("user-agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.90 Safari/537.36");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                var response = await httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode) throw new Exception(Convert.ToInt32(response.StatusCode).ToString());

                return await response.Content.ReadAsStringAsync();
            }
        }

        internal string GenerateURL(Filter currentFilter)
        {
            switch (currentFilter.Type)
            {
                case "Chemistry Style":
                    return GenerateChemistrySearchURL(currentFilter);
                case "Player":
                    return GeneratePlayerSearchURL(currentFilter);
                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        protected string GenerateChemistrySearchURL(Filter currentFilter)
        {
            var url = $"https://utas.external.s3.fut.ea.com/ut/game/fifa20/transfermarket?start=0&num=21&type=training&cat=playStyle&playStyle={currentFilter.ID}";
            var bidPriceMin = new Random().Next(3, 13) * 50;
            url += $"&micr={bidPriceMin}";
            if (currentFilter.SearchPrice > 0) url += $"&maxb={currentFilter.SearchPrice}";
            return url;
        }

        protected string GeneratePlayerSearchURL(Filter currentPlayer)
        {
            var url = $"https://utas.external.s3.fut.ea.com/ut/game/fifa20/transfermarket?start=0&num=21&type=player&maskedDefId={currentPlayer.ID}";
            var bidPriceMin = new Random().Next(3, 13) * 50;

            if (currentPlayer.IsSpecial) url += $"&rare=SP";

            url += $"&micr={bidPriceMin}";

            if (currentPlayer.SearchPrice > 0)
            {
                url += $"&macr={CalculateBid.CalculatePreviousBid(currentPlayer.SearchPrice)}";
                if (currentPlayer.MinPrice > 0) url += $"&minb={currentPlayer.MinPrice}";
                url += $"&maxb={currentPlayer.SearchPrice}";
                return url;
            }
            
            //In-form card first search
            if (currentPlayer.MaxPrice > 0) return url += $"&macr={CalculateBid.CalculatePreviousBid(currentPlayer.MaxPrice)}&minb={currentPlayer.MinPrice}&maxb={currentPlayer.MaxPrice}";

            //Non in-form first search
            return url;
        }
    }
}
