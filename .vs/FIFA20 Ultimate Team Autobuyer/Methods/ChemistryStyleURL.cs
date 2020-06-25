using FIFA20_Ultimate_Team_AutoBuyer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIFA20_Ultimate_Team_AutoBuyer.Methods
{
    public class ChemistryStyleURL
    {
        private readonly string baseURL = $"https://utas.external.s3.fut.ea.com/ut/game/fifa20/transfermarket?start=0&num=21&type=training&cat=playStyle";

        public string Generate(Filter filter)
        {
            var sb = new StringBuilder(baseURL);
            sb.Append(AppendChemistryStyleID(filter.ID));
            sb.Append(AppendSearchPrice(filter.SearchPrice));
            return sb.ToString();
        }

        private string AppendChemistryStyleID(int chemistryStyleID)
        {
            return $"&playStyle={chemistryStyleID}";
        }

        private string AppendSearchPrice(int searchPrice)
        {
            if (searchPrice == 0) return "";

            var minBidPrice = new Random().Next(3, 13) * 50;
            return $"&micr={minBidPrice}&maxb={searchPrice}";
        }
    }
}
