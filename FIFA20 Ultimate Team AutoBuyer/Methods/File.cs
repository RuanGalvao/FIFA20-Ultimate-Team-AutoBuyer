using FIFA20_Ultimate_Team_AutoBuyer.Models;
using FIFA20_Ultimate_Team_AutoBuyer.Tasks;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace FIFA20_Ultimate_Team_AutoBuyer.Methods
{
    public class File
    {
        private OpenFileDialog CreateOpenFileDialog(string title, string filter)
        {
            return new OpenFileDialog
            {
                Title = title,
                Filter = filter
            };
        }

        private string ReadFile(string fileName)
        {
            var sb = new StringBuilder();
            using (var streamReader = new StreamReader(fileName))
            {
                while (!streamReader.EndOfStream) sb.Append(streamReader.ReadLine()).Append("\n");
            }
            return sb.ToString();
        }

        private Filter ConvertLineToFilter(string line)
        {
            var elements = line.Split(',');
            return new Filter
            {
                Type = elements[0],
                ID = Convert.ToInt32(elements[1]),
                PlayerName = elements[0] == "Player" ? Player.GetName(Convert.ToInt32(elements[1])) : "N/A",
                Position = elements[2],
                Quality = elements[3],
                ChemistryStyle = elements[4],
                Rating = Convert.ToInt32(elements[5]),
                MinPrice = Convert.ToInt32(elements[6]),
                MaxPrice = Convert.ToInt32(elements[7]),
                Sell = elements[8] == "True" ? true : false
            };
        }

        private ObservableCollection<Filter> ConvertToFilters(string contents)
        {
            var filters = new ObservableCollection<Filter>();
            foreach (var line in contents.Split('\n'))
            {
                filters.Add(ConvertLineToFilter(line));
            }
            return filters;
        }

        public void LoadFilter(viewModel viewModel)
        {
            try
            {
                if (viewModel.IsConnected) throw new HandledException("Application cannot be running while adding a Filter");
                var openFileDialog = CreateOpenFileDialog("Load Filter", "CSV Files (*.csv)|*.csv");
                if (openFileDialog.ShowDialog() != DialogResult.OK) return;
                var fileContents = ReadFile(openFileDialog.FileName);
                var s = ConvertToFilters(fileContents);
                viewModel.SearchFilters = s;
            }
            catch (HandledException ex)
            {
                MessageBox.Show(ex.Message, Declarations.APPLICATION_NAME);
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to load filter", Declarations.APPLICATION_NAME);
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

        public void SaveFilter(string data)
        {
            try
            {
                var saveFileDialog = CreateSaveFileDialog("Save Filter", "CSV Files (*.csv)|*.csv");
                if (saveFileDialog.ShowDialog() != DialogResult.OK) return;
                System.IO.File.WriteAllText(saveFileDialog.FileName, data);
                MessageBox.Show("Filter saved successfully", Declarations.APPLICATION_NAME);
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to save filter", Declarations.APPLICATION_NAME);
            }
        }
    }
}
