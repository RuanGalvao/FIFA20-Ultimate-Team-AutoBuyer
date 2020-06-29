using FIFA20_Ultimate_Team_AutoBuyer.Methods;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;

namespace FIFA20_Ultimate_Team_AutoBuyer
{
    public class Validate
    {
        private readonly viewModel ViewModel;

        public Validate(viewModel viewModel)
        {
            ViewModel = viewModel;
        }

        private bool IsSessionIDValid()
        {
            var valid = !string.IsNullOrEmpty(ViewModel.SessionID);
            if (!valid) FifaMessageBox.Show("Please enter a Session ID");
            return valid;
        }

        private bool FilterContainsElements(ObservableCollection<IMarketplaceItem> filters)
        {
            var valid = filters.Count > 0;
            if (!valid) FifaMessageBox.Show("Please add a Filter to the list");
            return valid;
        }

        public bool AllowStart()
        {
            if (!IsSessionIDValid()) return false;
            if (!FilterContainsElements(ViewModel.MarketplaceItems)) return false;  
            return true;
        }
        public bool AllowFilterRemoval()
        {
            var valid = !(ViewModel.MarketplaceItems.Count == 1 && ViewModel.IsConnected);
            if (!valid) FifaMessageBox.Show("Player list cannot be empty while searching");
            return valid;
        }

        public bool AllowAdd()
        {
            if (ViewModel.SelectedType == Declarations.PLAYER) return PlayerAdd();
            if (ViewModel.SelectedType == Declarations.CHEMISTRY_STYLE) return ChemistryStyleAdd();
            throw new InvalidOperationException("Invalid Selected Type");
        }

        public bool ChemistryStyleAdd()
        {
            if (ChemistryStyleFilterExists()) return false;
            if (!IsPriceRangeValid()) return false;
            if (!MinPriceLessThanMaxPrice()) return false;
            return true;
        }

        private bool ChemistryStyleFilterExists()
        {
            var exists = ViewModel.MarketplaceItems.Any(p => p.FriendlyName == ViewModel.SelectedChemistryStyle);
            if (exists) FifaMessageBox.Show($"{ViewModel.SelectedChemistryStyle} has already been added");
            return exists;
        }

        private bool PlayerFilterExists()
        {
            var exists = ViewModel.MarketplaceItems.Any(p => p.FriendlyName == ViewModel.SelectedPlayer.Substring(0, ViewModel.SelectedPlayer.Length - 2) + ViewModel.SelectedRating);
            if (exists) FifaMessageBox.Show($"{ViewModel.SelectedPlayer} has already been added");
            return exists;
        }

        private bool PlayerExistsInFilter()
        {
            var exists = ViewModel.Players.Any(list => list.Contains(ViewModel.SelectedPlayer));
            if (!exists) FifaMessageBox.Show("Unknow Player entered");
            return exists;
        }

        private bool IsRatingSpecified()
        {
            if (ViewModel.SelectedQuality != "Special") return true;
            var valid = ViewModel.SelectedRating > 0;
            if (!valid) FifaMessageBox.Show("A rating must be specified for special cards");
            return valid;
        }

        private bool IsRatingValueValid()
        {
            if (ViewModel.SelectedQuality != "Special") return true;
            var valid = ViewModel.SelectedRating <= 99 && ViewModel.SelectedRating > 0;
            if (!valid) FifaMessageBox.Show("Player rating must be between 1-99");
            return valid;
        }

        private bool IsRatingValid()
        {
            if (!IsRatingSpecified()) return false;
            if (!IsRatingValueValid()) return false;
            return true;
        }

        private bool IsPriceSpecified()
        {
            var valid = !((ViewModel.SelectedMinPrice == 0 || ViewModel.SelectedMaxPrice == 0) && ViewModel.SelectedQuality == "Special");
            if (!valid) FifaMessageBox.Show("Please specify min and max price");
            return valid;
        }

        private bool IsPriceRangeValid()
        {
            var valid = ViewModel.SelectedMinPrice < 10000000 && ViewModel.SelectedMaxPrice < 10000000;
            if (!valid) FifaMessageBox.Show($"{ViewModel.SelectedType} price range must be between 0-9,999,999");
            return valid;
        }

        private bool MinPriceLessThanMaxPrice()
        {
            var valid = !((ViewModel.SelectedMinPrice > ViewModel.SelectedMaxPrice) && ViewModel.SelectedMinPrice != 0);
            if (!valid) FifaMessageBox.Show($"{ViewModel.SelectedType} min price cannot exceed max price");
            return valid;
        }

        private bool IsPriceValid()
        {
            if (!IsPriceSpecified()) return false;
            if (!IsPriceRangeValid()) return false;
            if (!MinPriceLessThanMaxPrice()) return false;
            return true;
        }

        private bool PlayerRatingLessThanOverrideRating()
        {
            var valid = !((ViewModel.SelectedOriginalRating > ViewModel.SelectedRating) && ViewModel.SelectedRating != 0);
            if (!valid) FifaMessageBox.Show("Invalid Rating entered");
            return valid;
        }

        private bool IsQualityValid()
        {
            if (!IsQualityRatingComboValid()) return false;
            if (!PlayerRatingLessThanOverrideRating()) return false;
            return true;
        }

        private bool IsPlayerNameEntered()
        {
            var valid = ViewModel.SelectedPlayer.Length > 0;
            if (!valid) FifaMessageBox.Show("Please enter a Player from the drop-down");
            return valid;
        }

        public bool PlayerAdd()
        {
            if (!IsPlayerNameEntered()) return false;
            if (PlayerFilterExists()) return false;
            if (!PlayerExistsInFilter()) return false;
            if (!IsRatingValid()) return false;
            if (!IsPriceValid()) return false;
            if (!IsQualityValid()) return false;
            return true;
        }

        private bool IsQualityRatingComboValid()
        {
            if (string.IsNullOrEmpty(ViewModel.SelectedQuality)) return true;
            if (ViewModel.SelectedQuality == "Special" && ViewModel.SelectedRating > 75) return true;
            if (ViewModel.SelectedQuality == "Gold" && ViewModel.SelectedRating >= 75) return true;
            if (ViewModel.SelectedQuality == "Silver" && ViewModel.SelectedRating >= 65 && ViewModel.SelectedRating <= 74) return true;
            if (ViewModel.SelectedQuality == "Bronze" && ViewModel.SelectedRating < 65) return true;
            FifaMessageBox.Show("Invalid Quality entered");
            return false;
        }
    }
}
