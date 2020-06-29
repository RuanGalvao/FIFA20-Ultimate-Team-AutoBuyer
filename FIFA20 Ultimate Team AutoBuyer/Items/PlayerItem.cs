namespace FIFA20_Ultimate_Team_AutoBuyer
{
    public class PlayerItem : IMarketplaceItem
    {
        public int Rating { get; set; }
        public string FriendlyName { get; set; }
        public string Quality { get; set; }
        public bool Sell { get; set; }
        public string Position { get; set; }
        public string ChemistryStyle { get; set; }
        public int Id { get; set; }
        public bool IsLegend { get; set; }
        public int SearchPrice { get; set; }
        public string ItemType { get => Declarations.PLAYER; }
        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }
    }
}
