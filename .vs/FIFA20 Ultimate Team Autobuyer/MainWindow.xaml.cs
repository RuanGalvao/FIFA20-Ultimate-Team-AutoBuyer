using FIFA20_Ultimate_Team_AutoBuyer.Models;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MessageBox = System.Windows.Forms.MessageBox;
using FIFA20_Ultimate_Team_AutoBuyer.Tasks;
using FIFA20_Ultimate_Team_AutoBuyer.Workers;
using File = FIFA20_Ultimate_Team_AutoBuyer.Methods.File;
using FIFA20_Ultimate_Team_AutoBuyer.Methods;

namespace FIFA20_Ultimate_Team_AutoBuyer
{
    public partial class MainWindow : Window
    {
        private readonly viewModel ViewModel = new viewModel();
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
                            if (ex.ForceDisconnect) MessageBox.Show(ex.Message, Declarations.APPLICATION_NAME);
                            ViewModel.IsConnected = !ex.ForceDisconnect;
                            addDelay = new TimeSpan(0, ex.Delay, 0);
                            if (ex.ClearSessionID) ViewModel.SessionID = "";
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, Declarations.APPLICATION_NAME);
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
            var file = new File();
            file.LoadFilter(ViewModel);
        }

        private void saveFilter_Click(object sender, RoutedEventArgs e)
        {
            var file = new File();
            file.SaveFilter(ViewModel.ToString());
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!new Validate().AllowStart(ViewModel)) return;
            if (!ViewModel.IsConnected) nextRunTime = DateTime.Now;
            ViewModel.IsConnected = !ViewModel.IsConnected;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (new Validate().AllowAdd(ViewModel)) new General(ViewModel).AddFilter();
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (new Validate().AllowFilterRemoval(ViewModel)) ViewModel.SearchFilters.Remove((Filter)DataGridPlayers1.SelectedItem);
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
            txtMinPrice.Text = Convert.ToString(new General(ViewModel).CalculateMinPrice(new Utils().ConvertToInt(txtMinPrice.Text)));
        }

        private void txtMaxPrice_LostFocus(object sender, RoutedEventArgs e)
        {
            txtMaxPrice.Text = Convert.ToString(new General(ViewModel).CalculateMaxPrice(new Utils().ConvertToInt(txtMaxPrice.Text)));
        }

        private void txtRating_LostFocus(object sender, RoutedEventArgs e)
        {
            var value = new Utils().ConvertToInt(txtRating.Text);
            if (value > 99) txtRating.Text = "99";
        }
    }
}