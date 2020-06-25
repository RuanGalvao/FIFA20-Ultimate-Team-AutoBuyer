using FIFA20_Ultimate_Team_AutoBuyer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIFA20_Ultimate_Team_AutoBuyer.Methods
{
    public class PlayerURL
    {
        private readonly string baseURL = $"https://utas.external.s3.fut.ea.com/ut/game/fifa20/transfermarket?";

        public string Generate(Filter filter, int pageNumber = 0)
        {
            var sb = new StringBuilder(baseURL);
            sb.Append(GeneratePageNumber(pageNumber));
            sb.Append(GenerateType(filter.Type));
            sb.Append(GeneratePlayerID(filter.ID));
            sb.Append(GeneratePosition(filter.Position));
            sb.Append(GenerateQuality(filter.Quality));
            sb.Append(GenerateChemistryStyle(filter.ChemistryStyle));
            sb.Append(GenerateSearchPrice(filter.MinPrice, filter.MaxPrice, filter.SearchPrice));
            return sb.ToString();
        }

        private string GeneratePageNumber(int pageNumber = 0)
        {
            return $"start={pageNumber * 20}&num=21";
        }

        private string GenerateType(string type)
        {
            return $"&type={type.ToLower()}";
        }

        private string GeneratePlayerID(int playerID)
        {
            return $"&maskedDefId={playerID}";
        }

        private string GenerateChemistryStyle(string chemistryStyle)
        {
            return "";
        }

        private string GenerateSearchPrice(int minBuyNowPrice, int maxBuyNowPrice, int searchPrice)
        {
            if (searchPrice == 0)
            {
                if (minBuyNowPrice > 0 || maxBuyNowPrice > 0) return $"&minb={minBuyNowPrice}&maxb={maxBuyNowPrice}";
                return "";
            } 

            var minBidPrice = new Random().Next(3, 13) * 50;
            
            return $"&micr={minBidPrice}&minb={minBuyNowPrice}&maxb={searchPrice}";
        }

        private string GeneratePosition(string position)
        {
            if (string.IsNullOrEmpty(position)) return "";
            if (position == "Attackers") return "&zone=attacker";
            if (position == "Midfielders") return "&zone=midfield";
            if (position == "Defenders") return "&zone=defense";
            return $"&pos={position}";
        }

        private string GenerateQuality(string quality)
        {
            if (string.IsNullOrEmpty(quality)) return "";
            if (quality == "Special") return "&rare=SP";
            return $"&lev={quality.ToLower()}";
        }
    }
}
