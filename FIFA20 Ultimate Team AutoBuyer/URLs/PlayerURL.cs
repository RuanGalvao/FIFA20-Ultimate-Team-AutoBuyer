using FIFA20_Ultimate_Team_AutoBuyer.URL;
using System.Text;

namespace FIFA20_Ultimate_Team_AutoBuyer.Methods
{
    public class PlayerURL : IMarketplaceURL
    {
        private readonly string baseURL = $"https://utas.external.s3.fut.ea.com/ut/game/fifa20/transfermarket?";
        private readonly PlayerItem PlayerItem;

        public PlayerURL(PlayerItem playerItem)
        {
            PlayerItem = playerItem;
        }

        public string GenerateUsingPageNumber(int pageNumber)
        {
            var sb = new StringBuilder(baseURL);
            sb.Append(GeneratePageNumber(pageNumber));
            sb.Append(AppendItemType());
            sb.Append(AppendID());
            sb.Append(AppendPostion());
            sb.Append(AppendQuality());
            sb.Append(AppendChemistryStyle());
            sb.Append(AppendSearchPrice());
            return sb.ToString();
        }

        public string Generate()
        {
            var sb = new StringBuilder(baseURL);
            sb.Append(GeneratePageNumber(0));
            sb.Append(AppendItemType());
            sb.Append(AppendID());
            sb.Append(AppendPostion());
            sb.Append(AppendQuality());
            sb.Append(AppendChemistryStyle());
            sb.Append(AppendSearchPrice());
            return sb.ToString();
        }

        private string GeneratePageNumber(int pageNumber)
        {
            return $"start={pageNumber * 20}&num=21";
        }

        private string AppendItemType()
        {
            return $"&type=player";
        }

        private string AppendID()
        {
            return $"&maskedDefId={PlayerItem.Id}";
        }

        private string AppendChemistryStyle()
        {
            return string.IsNullOrEmpty(PlayerItem.ChemistryStyle) ? "" : $"&playStyle={ChemistryStyle.GetID(PlayerItem.ChemistryStyle)}";
        }

        private string AppendSearchPrice()
        {
            if (PlayerItem.SearchPrice == 0 && PlayerItem.MinPrice == 0 && PlayerItem.MaxPrice == 0)
                return "";

            if (PlayerItem.SearchPrice == 0 && (PlayerItem.MinPrice > 0 && PlayerItem.MaxPrice > 0))
                return $"&minb={PlayerItem.MinPrice}&maxb={PlayerItem.MaxPrice}";

            return $"&minb={PlayerItem.MinPrice}&maxb={PlayerItem.SearchPrice}";
        }

        private string AppendPostion()
        {
            if (string.IsNullOrEmpty(PlayerItem.Position)) return "";
            if (PlayerItem.Position == "Attackers") return "&zone=attacker";
            if (PlayerItem.Position == "Midfielders") return "&zone=midfield";
            if (PlayerItem.Position == "Defenders") return "&zone=defense";
            return $"&pos={PlayerItem.Position}";
        }

        private string AppendQuality()
        {
            if (string.IsNullOrEmpty(PlayerItem.Quality)) return "";
            if (PlayerItem.Quality == "Special") return "&rare=SP";
            return $"&lev={PlayerItem.Quality.ToLower()}";
        }
    }
}
