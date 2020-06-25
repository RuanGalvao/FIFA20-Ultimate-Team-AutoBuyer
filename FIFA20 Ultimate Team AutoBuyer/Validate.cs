using FIFA20_Ultimate_Team_AutoBuyer.Methods;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;

namespace FIFA20_Ultimate_Team_AutoBuyer
{
    public class Validate
    {
        private bool IsSessionIDValid(string sessionID)
        {
            var valid = string.IsNullOrEmpty(sessionID);
            if (!valid) MessageBox.Show("Please enter a Session ID");
            return valid;
        }

        private bool FilterContainsElements(ObservableCollection<Models.Filter> filters)
        {
            var valid = filters.Count == 0;
            if (!valid) MessageBox.Show("Please add a Filter to the list");
            return valid;
        }

        public bool AllowStart(viewModel viewModel)
        {
            if (!IsSessionIDValid(viewModel.SessionID)) return false;
            if (!FilterContainsElements(viewModel.SearchFilters)) return false;  
            return true;
        }
        public bool AllowFilterRemoval(viewModel viewModel)
        {
            var valid = !(viewModel.SearchFilters.Count == 1 && viewModel.IsConnected);
            if (!valid) MessageBox.Show("Player list cannot be empty while searching", Declarations.APPLICATION_NAME);
            return valid;
        }

        public bool AllowAdd(viewModel viewModel)
        {
            if (viewModel.SelectedType == Declarations.PLAYER) return PlayerAdd(viewModel);
            if (viewModel.SelectedType == Declarations.CHEMISTRY_STYLE) return ChemistryStyleAdd(viewModel);
            throw new Exception("Invalid Selected Type");
        }

        public bool ChemistryStyleAdd(viewModel viewModel)
        {
            var utils = new Utils();
            if (IsTypeEntered(Declarations.CHEMISTRY_STYLE, viewModel.SelectedChemistryStyle)) return false;
            if (ChemistryStyleFilterExists(viewModel)) return false;
            if (PriceExceedsMaxValue(utils.ConvertToInt(viewModel.PlayerMinPrice), utils.ConvertToInt(viewModel.PlayerMaxPrice), Declarations.CHEMISTRY_STYLE)) return false;
            if (MinPriceExceedsMaxPrice(utils.ConvertToInt(viewModel.PlayerMinPrice), utils.ConvertToInt(viewModel.PlayerMaxPrice), Declarations.CHEMISTRY_STYLE)) return false;
            return true;
        }

        private bool IsTypeEntered(string type, string input)
        {
            if (string.IsNullOrEmpty(input)) MessageBox.Show($"Please select a {type} from the drop-down", Declarations.APPLICATION_NAME);
            return string.IsNullOrEmpty(input);
        }

        private bool ChemistryStyleFilterExists(viewModel viewModel)
        {
            var exists = viewModel.SearchFilters.Any(p => p.PlayerName == viewModel.SelectedChemistryStyle);
            if (exists) MessageBox.Show($"{viewModel.SelectedChemistryStyle} has already been added", Declarations.APPLICATION_NAME);
            return exists;
        }

        private bool PlayerFilterExists(viewModel viewModel)
        {
            var exists = viewModel.SearchFilters.Any(p => p.GetFriendlyName == viewModel.SelectedPlayer.Substring(0, viewModel.SelectedPlayer.Length - 2) + viewModel.SelectedRating);
            if (exists) MessageBox.Show($"{viewModel.SelectedPlayer} has already been added", Declarations.APPLICATION_NAME);
            return exists;
        }

        private bool PlayerExistsInFilter(viewModel viewModel)
        {
            var exists = viewModel.Players.Any(list => list.Contains(viewModel.SelectedPlayer));
            if (!exists) MessageBox.Show("Unknow Player entered", Declarations.APPLICATION_NAME);
            return exists;
        }

        private bool IsRatingSpecified(int rating, string quality)
        {
            var valid = !(quality == "Special" && rating > 0);
            if (!valid) MessageBox.Show("A rating must be specified for special cards");
            return valid;
        }

        private bool IsRatingValueValid(int rating)
        {
            var valid = (rating <= 99 && rating > 0);
            if (!valid) MessageBox.Show("Player rating must be between 1-99");
            return valid;
        }

        private bool IsRatingValid(int rating, string quality)
        {
            if (!IsRatingSpecified(rating, quality)) return false;
            if (!IsRatingValueValid(rating)) return false;
            return true;
        }

        private bool PriceSpecified(int minPrice, int maxPrice, string quality)
        {
            var valid = !((minPrice == 0 || maxPrice == 0) && quality == "Special");
            if (!valid) MessageBox.Show("Please specify min and max price");
            return valid;
        }

        private bool PriceExceedsMaxValue(int minPrice, int maxPrice, string type)
        {
            var valid = minPrice < 10000000 & maxPrice < 10000000;
            if (!valid) MessageBox.Show($"{type} price range must be between 0-9,999,999");
            return valid;
        }

        private bool MinPriceExceedsMaxPrice(int minPrice, int maxPrice, string type)
        {
            var valid = minPrice < maxPrice && maxPrice != 0;
            if (!valid) MessageBox.Show($"{type} min price cannot exceed max price");
            return valid;
        }

        private bool IsPriceValid(viewModel viewModel)
        {
            var utils = new Utils();
            if (PriceSpecified(utils.ConvertToInt(viewModel.PlayerMinPrice), utils.ConvertToInt(viewModel.PlayerMaxPrice), viewModel.SelectedQuality)) return false;
            if (PriceExceedsMaxValue(utils.ConvertToInt(viewModel.PlayerMinPrice), utils.ConvertToInt(viewModel.PlayerMaxPrice), viewModel.SelectedType)) return false;
            if (MinPriceExceedsMaxPrice(utils.ConvertToInt(viewModel.PlayerMinPrice), utils.ConvertToInt(viewModel.PlayerMaxPrice), viewModel.SelectedType)) return false;
            return true;
        }

        private bool OverrideRatingLessThanOriginalRating(int originalRating, int overrideRating)
        {
            var valid = overrideRating > originalRating && overrideRating > 0;
            if (!valid) MessageBox.Show("Invalid Rating entered");
            return valid;
        }

        private bool IsQualityValid(viewModel viewModel)
        {
            var utils = new Utils();
            var originalRating = utils.ConvertToInt(viewModel.SelectedPlayer.Substring(viewModel.SelectedPlayer.Length - 2, 2));
            var overrideRating = utils.ConvertToInt(viewModel.SelectedRating);

            if (!IsQualityRatingComboValid(viewModel.SelectedQuality, overrideRating > 0 ? overrideRating : originalRating)) return false;
            if (OverrideRatingLessThanOriginalRating(originalRating, overrideRating)) return false;
            return true;
        }

        public bool PlayerAdd(viewModel viewModel)
        {
            if (IsTypeEntered(Declarations.PLAYER, viewModel.SelectedPlayer)) return false;
            if (PlayerFilterExists(viewModel)) return false;
            if (!PlayerExistsInFilter(viewModel)) return false;
            if (!IsRatingValid(Convert.ToInt32(viewModel.SelectedRating), viewModel.SelectedQuality)) return false;
            if (!IsPriceValid(viewModel)) return false;
            if (!IsQualityValid(viewModel)) return false;
            return true;
        }

        private bool IsQualityRatingComboValidSpecial(string quality, int rating)
        {
            var valid = (quality == "Special" || string.IsNullOrEmpty(quality)) && rating >= 75;
            if (!valid) MessageBox.Show("Invalid Quality entered");
            return valid;
        }

        private bool IsQualityRatingComboValidGold(string quality, int rating)
        {
            var valid = rating >= 75 && quality != "Gold";
            if (!valid) MessageBox.Show("Invalid Quality entered");
            return valid;
        }

        private bool IsQualityRatingComboValidSilver(string quality, int rating)
        {
            var valid = rating >= 65 && rating <= 74 && quality != "Silver";
            if (!valid) MessageBox.Show("Invalid Quality entered");
            return valid;
        }

        private bool IsQualityRatingComboValidBronze(string quality, int rating)
        {
            var valid = rating <= 64 && quality != "Bronze";
            if (!valid) MessageBox.Show("Invalid Quality entered");
            return valid;
        }

        private bool IsQualityRatingComboValid(string quality, int rating)
        {
            if (!IsQualityRatingComboValidSpecial(quality, rating)) return false;
            if (!IsQualityRatingComboValidGold(quality, rating)) return false;
            if (!IsQualityRatingComboValidSilver(quality, rating)) return false;
            if (!IsQualityRatingComboValidBronze(quality, rating)) return false;
            return true;
        }
    }
}
