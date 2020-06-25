using System;
using System.Collections.Generic;

namespace FIFA20_Ultimate_Team_AutoBuyer.Models
{
    public class ItemData
    {
        public int AssetId { get; set; }
        public ushort CardSubTypeId { get; set; }
        public long Id { get; set; }
        public string ItemType { get; set; }
        public byte Rating { get; set; }
        public long ResourceId { get; set; }
    }
}
