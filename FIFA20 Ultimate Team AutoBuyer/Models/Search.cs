using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIFA20_Ultimate_Team_Autobuyer.Models
{

    [Serializable]
    public class Search
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public int SearchPrice { get; set; }
        public int Rating { get; set; }
    }
}
