using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIFA20_Ultimate_Team_AutoBuyer.Enums
{
    public enum FIFAUltimateTeamStatusCode
    {
        InsufficientFunds = 470,
        OK = 200,
        BadRequest = 400,
        Unauthorized = 401,
        NotFound = 404,
        UpgradeRequired = 426,
        InternalServerError = 500,
        CaptureRequired = 458,
        Sold = 461,
        RetryAfter = 512,
        TooManyRequests = 429
    }
     
}
