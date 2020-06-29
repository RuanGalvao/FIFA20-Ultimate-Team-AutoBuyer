using FIFA20_Ultimate_Team_AutoBuyer.Tasks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace FIFA20_Ultimate_Team_AutoBuyer.Methods
{
    public class File
    {
        private readonly viewModel ViewModel;

        public File(viewModel viewModel)
        {
            ViewModel = viewModel;
        }

        private OpenFileDialog CreateOpenFileDialog(string title, string filter)
        {
            return new OpenFileDialog
            {
                Title = title,
                Filter = filter
            };
        }

        private string GetFileContentsToString(string fileName)
        {
            return System.IO.File.ReadAllText(fileName);
        }

        private IMarketplaceItem ConvertLineToMarketplaceItem(string line)
        {
            var elements = line.Split(',');

            if (elements[0] == Declarations.PLAYER)
            {
                return new PlayerItem
                {
                    FriendlyName = Player.GetName(Convert.ToInt32(elements[1])),
                    Id = Convert.ToInt32(elements[1]),
                    Position = elements[2],
                    Quality = elements[3],
                    ChemistryStyle = elements[4],
                    Rating = Convert.ToInt32(elements[5]),
                    MinPrice = Convert.ToInt32(elements[6]),
                    MaxPrice = Convert.ToInt32(elements [7]),
                    Sell = elements[8] == "True" ? true : false
                };
            }
            else if (elements[0] == Declarations.CHEMISTRY_STYLE)
            {
                return new ChemistryStyleItem
                {
                    FriendlyName = ChemistryStyle.GetName(Convert.ToInt32(elements[1])),
                    Id = Convert.ToInt32(elements[1]),
                    Quality = elements[3],
                    Rating = Convert.ToInt32(elements[5]),
                    MinPrice = Convert.ToInt32(elements[6]),
                    MaxPrice = Convert.ToInt32(elements[7]),
                    Sell = elements[8] == "True" ? true : false
                };
            }
            throw new InvalidOperationException();
        }

        private ObservableCollection<IMarketplaceItem> ConvertListToMarketplaceItems(string contents)
        {
            var marketplaceItems = new ObservableCollection<IMarketplaceItem>();
            var lines = contents.Split('\n');
            foreach (var line in lines) marketplaceItems.Add(ConvertLineToMarketplaceItem(line));
            return marketplaceItems;
        }

        private void GetAndSetMarketplaceItemsUsingOpenFileDialog()
        {
            var openFileDialog = CreateOpenFileDialog("Load Filter", "CSV Files (*.csv)|*.csv");
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;
            var fileContents = GetFileContentsToString(openFileDialog.FileName);
            ViewModel.MarketplaceItems = ConvertListToMarketplaceItems(fileContents);
        }

        public void LoadMarketplaceItems()
        {
            try
            {
                if (ViewModel.IsConnected) throw new HandledException("Application cannot be running while adding a Filter");
                GetAndSetMarketplaceItemsUsingOpenFileDialog();
            }
            catch (HandledException ex)
            {
                FifaMessageBox.Show(ex.Message);
            }
            catch (Exception)
            {
                FifaMessageBox.Show("Unable to load filter");
            }
        }

        private SaveFileDialog CreateSaveFileDialog(string title, string filter)
        {
            return new SaveFileDialog
            {
                Title = title,
                Filter = filter
            };
        }

        private void SaveStringToCSVUsingSaveFileDialog(string contents)
        {
            if (contents.Length == 0) throw new HandledException("No filters exist");
            var saveFileDialog = CreateSaveFileDialog("Save Filter", "CSV Files (*.csv)|*.csv");
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                System.IO.File.WriteAllText(saveFileDialog.FileName, contents);
                FifaMessageBox.Show("Filters saved successfully");
            }
            saveFileDialog.Dispose();
        }

        public void SaveMarketplaceItems()
        {
            try
            {
                SaveStringToCSVUsingSaveFileDialog(ViewModel.ToString());
            }
            catch (HandledException ex)
            {
                FifaMessageBox.Show(ex.Message);
            }
            catch (Exception)
            {
                FifaMessageBox.Show("Unable to save filters");
            }
        }
    }
}
