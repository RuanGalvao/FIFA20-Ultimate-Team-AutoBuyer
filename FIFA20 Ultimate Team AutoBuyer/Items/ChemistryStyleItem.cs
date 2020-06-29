using System;

namespace FIFA20_Ultimate_Team_AutoBuyer
{
    public class ChemistryStyleItem : IMarketplaceItem
    {
        public int Id { get; set; }

        public int Rating { get; set; }

        public string FriendlyName { get; set; }

        public string Quality { get; set; }

        public bool Sell { get; set; }

        public int SearchPrice { get; set; }
        public string ItemType { get => Declarations.CHEMISTRY_STYLE; }
        public int DefinitionId { get ; set ; }
        public int MinPrice { get ; set; }
        public int MaxPrice { get; set; }
    }
}
