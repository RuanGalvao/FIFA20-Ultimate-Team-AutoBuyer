using System;

namespace FIFA20_Ultimate_Team_AutoBuyer.Models
{
    public class Filter
    {
        public string PlayerName { get; set; }
        public bool IsLegend { get; set; }
        public string Position { get; set; }
        public string Quality { get; set; }
        public string ChemistryStyle { get; set; }
        public int Rating { get; set; }
        public int ID { get; set; }
        public string Type { get; set; }
        public int SearchPrice { get; set; }
        public bool IsSpecial { get; set; }
        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }
        public bool Sell { get; set; }

        public string GetFriendlyName => Type == "Chemistry Style" ? $"{ChemistryStyle}" : $"{PlayerName} {Rating}";
        public string GetPlayerName => PlayerName == "" ? "N/A" : PlayerName;
        public string GetPosition => Position == "" ? "N/A" : Position;
        public string GetRating => Rating == 0 ? "N/A" : Rating.ToString();
        public string GetChemistryStyle => ChemistryStyle == "" ? "N/A" : ChemistryStyle;
        public string GetMinPrice => MinPrice == 0 ? "N/A" : MinPrice.ToString();
        public string GetMaxPrice => MaxPrice == 0 ? "N/A" : MaxPrice.ToString();

        //public override string ToString()
        //{
        //    return String.Format($"{ID}{MinPrice}{MaxPrice}{Sell}{SearchPrice}");
        //}
    }
}
