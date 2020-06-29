using FIFA20_Ultimate_Team_AutoBuyer.URL;
using System;
using System.Text;

namespace FIFA20_Ultimate_Team_AutoBuyer.Methods
{
    public class ChemistryStyleURL : IMarketplaceURL
    {
        private readonly string baseURL = $"https://utas.external.s3.fut.ea.com/ut/game/fifa20/transfermarket?";
        private readonly ChemistryStyleItem ChemistryStyleItem;

        public ChemistryStyleURL(ChemistryStyleItem chemistryStyleItem)
        {
            ChemistryStyleItem = chemistryStyleItem;
        }

        private string GeneratePageNumber(int pageNumber)
        {
            return $"start={pageNumber * 20}&num=21";
        }

        public string GenerateUsingPageNumber(int pageNumber)
        {
            var sb = new StringBuilder(baseURL);
            sb.Append(GeneratePageNumber(pageNumber));
            sb.Append(AppendType());
            sb.Append(AppendID());
            sb.Append(AppendSearchPrice());
            return sb.ToString();
        }

        private string AppendType()
        {
            return "&type=training&cat=playStyle";
        }

        public string Generate()
        {
            var sb = new StringBuilder(baseURL);
            sb.Append(AppendID());
            sb.Append(AppendSearchPrice());
            return sb.ToString();
        }

        private string AppendID()
        {
            return $"&playStyle={ChemistryStyleItem.Id}";
        }

        private string AppendSearchPrice()
        {
            return ChemistryStyleItem.SearchPrice == 0 ? "" : $"&maxb={ChemistryStyleItem.SearchPrice}";
        }
    }
}
