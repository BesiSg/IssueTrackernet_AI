
using BesiAI;
using Handlers;
using Handlers.EventAggregator;
using System.Collections.ObjectModel;
using System.Timers;
using System.Windows;
using Utility;
using Utility.EventAggregator;
using Utility.Lib.ColumnDisplay;
using Utility.Lib.Filter;
using Utility.Lib.PathConfig;
using Utility.Lib.SettingHandler;
using Utility.Lib.Ticket;

namespace IssuelistModule.ViewModels
{
    public class IssuelistViewModel : ViewModelBase
    {
        IEventAggregator _ea;
        public ObservableCollection<TicketInfo> Data
        {
            get => GetValue(() => Data);
            set => SetValue(() => Data, value);
        }
        private List<TicketInfo> _Data = new List<TicketInfo>();
        public DelegateCommand PriorityChangedCommand { get; private set; }
        public AsyncDelegateCommand SummarizeCommand { get; private set; }
        public bool CanSummarize
        {
            get => GetValue(() => CanSummarize);
            set => SetValue(() => CanSummarize, value);
        }

        private SettingHandler<PathConfig> pathcfgHandler;
        private SettingHandler<Filters> filtercfgHandler;
        private SettingHandler<Dataset> datasetcfgHandler;
        private SettingHandler<AIDataset> aidatasetHandler;
        private AIHandler AIHandler;
        private DataSetHandler dataSetHandler;
        public PathConfig PathCfg => pathcfgHandler.Get;
        public Filters Filtercfg => filtercfgHandler.Get;
        public Dataset Datasetcfg => datasetcfgHandler.Get;
        public AIDataset Aidataset => aidatasetHandler.Get;
        public TicketInfo Selected
        {
            get => this.GetValue(() => this.Selected);
            set
            {
                if (Selected == null && value == null) return;
                if (Selected?.Equals(value) == true) return;
                this.SetValue(() => this.Selected, value);
                _ea.GetEvent<IssueSelectionChange>().Publish(Selected);
            }
        }

        public IssuelistViewModel(IEventAggregator ea, SettingHandler<PathConfig> pathcfg, SettingHandler<Filters> filtercfg, SettingHandler<Dataset> datasetcfg, SettingHandler<AIDataset> aiDatasetHandler, DataSetHandler setHandler, AIHandler handler)
        {
            _ea = ea;
            this.pathcfgHandler = pathcfg;
            this.filtercfgHandler = filtercfg;
            this.datasetcfgHandler = datasetcfg;
            this.aidatasetHandler = aiDatasetHandler;
            this.dataSetHandler = setHandler;
            AIHandler = handler;
            Data = new ObservableCollection<TicketInfo>();
            this.datasetcfgHandler.SettingLoaded += DatasetCfgHandler_SettingLoaded;
            this.filtercfgHandler.SettingLoaded += FilterCfgHandler_SettingLoaded;
            Filtercfg.SelectionChange += Filtercfg_SelectionChange;
            this.FilterDataSetToUI().Wait();
            PriorityChangedCommand = new DelegateCommand(() => this.dataSetHandler.SetPrio());
            _ea.GetEvent<IssuesListChanged>().Subscribe(GetUpdatedList);
            CanSummarize = true;
        }

        bool FilterChanged = false;
        System.Timers.Timer RefreshUITimer = null;
        private void Filtercfg_SelectionChange(object? sender, FilterItem e)
        {
            this.FilterChanged = true;
            if (this.RefreshUITimer != null)
            {
                this.TerminateTimer();
            }
            this.InitialiseTimer();
        }

        private async void GetUpdatedList(List<TicketInfo> list)
        {
            await FilterDataSetToUI();
        }

        private void TerminateTimer()
        {
            this.RefreshUITimer?.Stop();
            this.RefreshUITimer?.Close();
            this.RefreshUITimer = null;
        }
        private void InitialiseTimer()
        {
            this.RefreshUITimer = new System.Timers.Timer(1000);
            this.RefreshUITimer.Elapsed += RefreshUITimer_Elapsed;
            this.RefreshUITimer.Start();
        }
        private void RefreshUITimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.UpdateListIfFilterChange();
            this.TerminateTimer();
        }

        private void FilterCfgHandler_SettingLoaded(object sender, EventArgs e)
        {
        }

        private void DatasetCfgHandler_SettingLoaded(object sender, EventArgs e)
        {
            this.dataSetHandler.DatasetChanged += DatasetCfg_DatasetChanged;
            this.FilterDataSetToUI().Wait();
        }

        private void DatasetCfg_DatasetChanged(object sender, EventArgs e)
        {
            this.FilterDataSetToUI().Wait();
        }

        private void UpdateListIfFilterChange()
        {
            if (!this.FilterChanged) return;
            this.FilterDataSetToUI().Wait();
            this.FilterChanged = false;
        }

        public ColumnDisplay ColDisp => this.pathcfgHandler.Get.ColDisplay;
        public int IssueCounter
        {
            get => this.GetValue(() => this.IssueCounter);
            set => this.SetValue(() => this.IssueCounter, value);
        }
        private void Clear()
        {
            lock (_Data)
            {
                this._Data.Clear();
            }
        }
        private void Add(IEnumerable<TicketInfo> source)
        {
            ClearErrorFlags();
            try
            {
                if (source == null) return;
                lock (_Data)
                {
                    this._Data.AddRange(source);
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }
        private void Remove(IEnumerable<TicketInfo> source)
        {
            try
            {
                lock (_Data)
                {
                    source.ToList().ForEach(x => this._Data.Remove(x));
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private Task<ErrorResult> FilterDataSetToUI()
        {
            return Task.Run(() =>
            {
                ClearErrorFlags();
                try
                {
                    this.Clear();
                    var Task1 = this.Filtercfg.UpdateFilternGetListtoRemove();
                    this.Add(this.dataSetHandler.Get.GetValues());
                    Task.WaitAll(Task1);
                    this.Remove(Task1.Result.Result);
                    UpdateCollection();
                    this.FilterChanged = false;
                }
                catch (Exception ex)
                {
                    CatchException(ex);
                }
                return Result;
            });
        }
        private void UpdateCollection()
        {
            Application.Current.Dispatcher.BeginInvoke((Action)delegate // <--- HERE
            {
                this.Data.Clear();
                this.Data.AddRange(this._Data);
                this.IssueCounter = this.Data.Count;
            });
        }

        private async Task Summarize()
        {
            CanSummarize = false;
            await Task.Run(() =>
            {
                foreach (var ticket in Data)
                {
                    ticket.SummarizedStatus = AIHandler.GetAnswerAsync(ticket.Comments).Result.Item2;
                }
            });
            CanSummarize = true;
        }
    }
}
