using System.Threading.Tasks;

namespace FIFA20_Ultimate_Team_AutoBuyer.Workers
{
    public class WorkerHandler
    {
        private readonly viewModel ViewModel;
        private readonly SearchItemWorker SearchItemFetcher;
        private readonly RefreshTradePileWorker CheckTradePileWorker;

        public WorkerHandler(
            viewModel viewModel,  
            SearchItemWorker searchItemFetcher,
            RefreshTradePileWorker checkTradePileWorker
        )
        {
            ViewModel = viewModel;
            SearchItemFetcher = searchItemFetcher;
            CheckTradePileWorker = checkTradePileWorker;
        }

        public async Task RunWorkers()
        {
            await CheckTradePileWorker.DoWork();
            for (int i = 0; i < ViewModel.SearchFilters.Count; i++) await SearchItemFetcher.Resolve(ViewModel.SearchFilters[i]);
        }
    }
}
