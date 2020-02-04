using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIFA20_Ultimate_Team_Autobuyer.Methods
{
    public static class CalculateBid
    {
        public static int CalculateMinProfitMargin(int currentBid)
        {
            if (currentBid < 1000)
                return 50;

            if (currentBid < 10000)
                return 100;

            if (currentBid < 50000)
                return 250;

            if (currentBid < 100000)
                return 500;

            return 1000;
        }

        public static int CalculateNextBid(int currentBid)
        {
            if (currentBid <= 200)
                return 200;

            if (currentBid < 1000)
                return currentBid + 50;

            if (currentBid < 10000)
                return currentBid + 100;

            if (currentBid < 50000)
                return currentBid + 250;

            if (currentBid < 100000)
                return currentBid + 500;

            return currentBid + 1000;
        }

        public static int CalculatePreviousBid(int currentBid)
        {
            if (currentBid <= 200)
                return 200;

            if (currentBid <= 1000)
                return currentBid - 50;

            if (currentBid <= 10000)
                return currentBid - 100;

            if (currentBid <= 50000)
                return currentBid - 250;

            if (currentBid <= 100000)
                return currentBid - 500;

            return currentBid - 1000;
        }
    }
}
