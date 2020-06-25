using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIFA20_Ultimate_Team_AutoBuyer.Models
{
    public class TradePileGrid
    {
        public string Name { get; set; }

        public long Duration { get; set; }

        public int Rating { get; set; }
        public string Type { get; set; }

        public int BuyNowPrice { get; set; }

        public string Status { get; set; }

        public string Updates => LatestUpdate(Duration, Status);

        private string LatestUpdate(long duration, string status)
        {
            if (string.IsNullOrEmpty(status)) return "Available";
            if (status == "expired") return "Expired";
            if (duration < 0) return "Sold";

            // Active
            if (duration == 0) return "Unlisted";
            if (duration <= 3600) return (duration / 60).ToString() + "m";
            if (duration <= 86400) return (duration / 3600).ToString() + "h";
            return (duration / 86400).ToString() + "d";
        }
    }
}
