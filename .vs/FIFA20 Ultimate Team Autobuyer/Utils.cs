using System;

namespace FIFA20_Ultimate_Team_AutoBuyer.Methods
{
    public class Utils
    {
        public int ConvertToInt(string value)
        {
            try
            {
                return Convert.ToInt32(value);
            }
            catch
            {
                return 0;
            }
        }
    }
}
