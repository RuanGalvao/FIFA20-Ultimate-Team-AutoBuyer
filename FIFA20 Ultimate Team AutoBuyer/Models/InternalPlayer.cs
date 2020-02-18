using System;

namespace FIFA20_Ultimate_Team_Autobuyer.Models
{
    public class InternalPlayer
    {
        public string Name { get; set; }
        public bool IsLegend { get; set; }
        public int Rating { get; set; }
        public int ID { get; set; }
        public int SearchPrice { get; set; }
        public bool IsSpecial { get; set; }
        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }

        public string NameRating => $"{Name} {Rating}";
        public string MinMaxPrice => $"{MinPrice}-{MaxPrice}";
    }
}
