namespace FIFA20_Ultimate_Team_AutoBuyer
{
    public interface IMarketplaceItem
    {
        int Id { get; set; }
        int Rating { get; set; }
        string FriendlyName { get; set; }
        string Quality { get; set; }
        int MinPrice { get; set; }
        int MaxPrice { get; set; }
        bool Sell { get; set; }
        int SearchPrice { get; set; }
        string ItemType { get; }
    }
}
