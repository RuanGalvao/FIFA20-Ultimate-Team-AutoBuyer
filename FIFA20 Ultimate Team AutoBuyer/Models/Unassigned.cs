using System.Collections.Generic;

namespace FIFA20_Ultimate_Team_AutoBuyer.Models
{
    public class Unassigned
    {
        public IEnumerable<UnassignedModel> itemdata;

        public class UnassignedModel
        {
            public long id { get; set; }
        }
        
    }
}
