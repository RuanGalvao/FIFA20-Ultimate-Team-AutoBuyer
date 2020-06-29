
namespace FIFA20_Ultimate_Team_AutoBuyer.URL
{
    public interface IMarketplaceURL
    {
        public string GenerateUsingPageNumber(int pageNumber);

        public string Generate();
    }
}
