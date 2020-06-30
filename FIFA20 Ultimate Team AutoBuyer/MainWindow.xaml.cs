using FIFA20_Ultimate_Team_AutoBuyer.Models;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FIFA20_Ultimate_Team_AutoBuyer.Tasks;
using FIFA20_Ultimate_Team_AutoBuyer.Workers;
using File = FIFA20_Ultimate_Team_AutoBuyer.Methods.File;
using FIFA20_Ultimate_Team_AutoBuyer.Methods;

namespace FIFA20_Ultimate_Team_AutoBuyer
{
    public partial class MainWindow : Window
    {
        private readonly viewModel ViewModel = new viewModel();

        private readonly Validate Validate;
        private readonly General General;
        private readonly File File;

        private DateTime nextRunTime;
        private TimeSpan addDelay;

        public MainWindow()
        {
            DataContext = ViewModel;
            InitializeComponent();
            Title = Declarations.APPLICATION_NAME;

            var searchItemFetcher = new SearchItemWorker(ViewModel);
            var checkTradePileWorker = new RefreshTradePileWorker(ViewModel);
            var workerHandler = new WorkerHandler(ViewModel, searchItemFetcher, checkTradePileWorker);

            Validate = new Validate(ViewModel);
            General = new General(ViewModel);
            File = new File(ViewModel);

            nextRunTime = DateTime.Now;
            addDelay = new TimeSpan(0, 0, 0);

            var x = Task.Run(async () =>
            {
                while (true)
                {
                    if (ViewModel.IsConnected && DateTime.Now > nextRunTime)
                    {
                        try
                        {
                            await workerHandler.RunWorkers();
                        } 
                        catch (HandledException ex)
                        {
                            if (ex.ForceDisconnect) FifaMessageBox.Show(ex.Message);
                            ViewModel.IsConnected = !ex.ForceDisconnect;
                            addDelay = new TimeSpan(0, ex.Delay, 0);
                            if (ex.ClearSessionID) ViewModel.SessionID = "";
                        }
                        catch (Exception ex)
                        {
                            FifaMessageBox.Show(ex.Message);
                            ViewModel.IsConnected = false;
                        }

                        nextRunTime = DateTime.Now + new TimeSpan(0, 0, 5) + addDelay;
                        addDelay = new TimeSpan(0, 0, 0);
                    }
                    Thread.Sleep(100);
                }
            });
        }

        private void loadFilter_Click(object sender, RoutedEventArgs e)
        {
            File.LoadMarketplaceItems();
        }

        private void saveFilter_Click(object sender, RoutedEventArgs e)
        {
            File.SaveMarketplaceItems();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate.AllowStart()) return;
            if (!ViewModel.IsConnected) nextRunTime = DateTime.Now;
            ViewModel.IsConnected = !ViewModel.IsConnected;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (Validate.AllowAdd()) General.AddFilter();
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (Validate.AllowFilterRemoval()) ViewModel.MarketplaceItems.Remove((IMarketplaceItem)DataGridPlayers1.SelectedItem);
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            txtSessionID.Focus();
        }

        private void cboPlayer_KeyDown(object sender, KeyEventArgs e)
        {
            cboPlayer.IsDropDownOpen = true;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void txtMinPrice_LostFocus(object sender, RoutedEventArgs e)
        {

            txtMinPrice.Text = Convert.ToString(General.CalculateMinPrice(Convert.ToInt32(txtMinPrice.Text)));
        }

        private void txtMaxPrice_LostFocus(object sender, RoutedEventArgs e)
        {
            txtMaxPrice.Text = Convert.ToString(General.CalculateMaxPrice(Convert.ToInt32(txtMaxPrice.Text)));
        }

        private void txtRating_LostFocus(object sender, RoutedEventArgs e)
        {
            var value = Convert.ToInt32(txtRating.Text);
            if (value > 99) txtRating.Text = "99";
        }
    }
}