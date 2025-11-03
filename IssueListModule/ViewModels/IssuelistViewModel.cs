using Handlers;
using Handlers.EventAggregator;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Timers;
using System.Windows;
using Utility;
using Utility.Lib.ColumnDisplay;
using Utility.Lib.Filter;
using Utility.Lib.PathConfig;
using Utility.Lib.SettingHandler;
using Utility.Lib.Ticket;

namespace IssuelistModule.ViewModels
{
    public class IssuelistViewModel : BaseUtility
    {
        string StartupPath = AppDomain.CurrentDomain.BaseDirectory;
        string PathConfig_Filename = "Data\\Path.xml";
        string FilterConfig_Filename = "Data\\Filter.xml";
        string PathConfig_Path;
        string FilterConfig_Path;
        IEventAggregator _ea;
        public DelegateCommand<TicketInfo> SelectedItemChangedCommand { get; private set; }
        public DelegateCommand<string> PriorityChangedCommand { get; private set; }
        public DelegateCommand OnloadedCommand { get; private set; }
        public SettingHandler<PathConfig> PathCfgHandler { get; set; }
        public SettingHandler<Filters> FilterCfgHandler { get; set; }
        public SettingHandler<Dataset> DatasetCfgHandler { get; set; }

        public IssuelistViewModel(IEventAggregator ea, SettingHandler<PathConfig> pathcfg, SettingHandler<Filters> filtercfg, SettingHandler<Dataset> datasetcfg  )
        {
            _ea = ea;
            this.PathConfig_Path = Path.Combine(StartupPath, PathConfig_Filename);
            this.FilterConfig_Path = Path.Combine(StartupPath, FilterConfig_Filename);

            this.PathCfgHandler = pathcfg;
            this.FilterCfgHandler = filtercfg;
            this.DatasetCfgHandler = datasetcfg;

            this.PathCfgHandler.SetPathnLoad(PathConfig_Path);
            this.FilterCfgHandler.SetPathnLoad(FilterConfig_Path);
            this.DatasetCfgHandler.SetPathnLoad(this.PathCfg.DataPath);
            this.DatasetCfg = new DataSetHandler(this.DatasetCfgHandler.Get);
            this.UpdateVisList();
            this.LinkFilterDataset();
            this.PathCfgHandler.SettingLoaded += PathCfgHandler_SettingLoaded;
            this.PathCfg.LoadPathChanged += PathCfgHandler_SettingLoaded;
            this.PathCfg.SavePathChanged += PathCfgHandler_SettingSaved;
            this.DatasetCfgHandler.SettingLoaded += DatasetCfgHandler_SettingLoaded;
            this.FilterCfgHandler.SettingLoaded += FilterCfgHandler_SettingLoaded;
            this.DatasetCfg.DatasetChanged += DatasetCfg_DatasetChanged;
            this.FilterCfg.SelectionChange += FilterCfg_SelectionChange;
            this.FilterDataSetToUI().Wait();

            SelectedItemChangedCommand = new DelegateCommand<TicketInfo>((selectedItem) =>
            {
                _ea.GetEvent<IssueSelectionChange>().Publish(selectedItem);
            });
            PriorityChangedCommand = new DelegateCommand<string>((priorities) =>
            {
                this.DatasetCfg.SetPrio();
            });
            OnloadedCommand = new DelegateCommand(() =>
            {
                this.FiltersUpdated();
            });
        }
        bool FilterChanged = false;
        System.Timers.Timer RefreshUITimer = null;
        private void FilterCfg_SelectionChange(object sender, FilterItem e)
        {
            this.FilterChanged = true;
            if (this.RefreshUITimer != null)
            {
                this.TerminateTimer();
            }
            this.InitialiseTimer();
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
            this.LinkFilterDataset();
        }

        private void DatasetCfgHandler_SettingLoaded(object sender, EventArgs e)
        {
            this.DatasetCfg.DatasetChanged += DatasetCfg_DatasetChanged;
            this.LinkFilterDataset();
            this.FilterDataSetToUI().Wait();
        }

        private void DatasetCfg_DatasetChanged(object sender, EventArgs e)
        {
            this.FilterDataSetToUI().Wait();
        }

        private void PathCfgHandler_SettingSaved(object sender, EventArgs e)
        {
            this.DatasetCfgHandler.SetPathnSave(this.PathCfg.DataPath);
        }

        private void PathCfgHandler_SettingLoaded(object sender, EventArgs e)
        {
            this.DatasetCfgHandler.SetPathnLoad(this.PathCfg.DataPath);
            this.UpdateVisList();
            this.NeedRebind?.Invoke(this, null);
        }

        private void FiltersUpdated()
        {
            _ea.GetEvent<FiltersChanged>().Publish(FilterCfg);
        }
        public void Save()
        {
            this.PathCfgHandler.Save();
            this.FilterCfgHandler.Save();
            this.DatasetCfgHandler.Save();
        }

        public void UpdateListIfFilterChange()
        {
            if (!this.FilterChanged) return;
            this.FilterDataSetToUI().Wait();
            this.FilterChanged = false;
        }

        public event EventHandler NeedRebind;
        public DataSetHandler DatasetCfg { get; private set; }
        private List<TicketInfo> _Collection = new List<TicketInfo>();
        public ObservableCollection<TicketInfo> Collection { get; private set; } = new ObservableCollection<TicketInfo>();
        public ObservableCollection<NameBoolPair> VisList { get; private set; } = new ObservableCollection<NameBoolPair>();
        public Filters FilterCfg => this.FilterCfgHandler.Get;
        public PathConfig PathCfg => this.PathCfgHandler.Get;
        public ColumnDisplay ColDisp => this.PathCfgHandler.Get.ColDisplay;
        public int IssueCounter
        {
            get => this.GetValue(() => this.IssueCounter);
            set => this.SetValue(() => this.IssueCounter, value);
        }
        private void UpdateCollection()
        {
            Application.Current.Dispatcher.BeginInvoke((Action)delegate // <--- HERE
            {
                this.Collection.Clear();
                this.Collection.AddRange(this._Collection);
                this.IssueCounter = this._Collection.Count;
            });
        }
        private void UpdateVisList()
        {
            Application.Current.Dispatcher.BeginInvoke((Action)delegate // <--- HERE
            {
                this.VisList.Clear();
                this.VisList.AddRange(this.PathCfg.GetVisList());
            });
        }
        private void LinkFilterDataset()
        {
            this.FilterCfg.SetDataset(this.DatasetCfgHandler.Get);
        }
        private void Clear()
        {
            this._Collection.Clear();
        }
        private Task<ErrorResult> Add(IEnumerable<TicketInfo> source)
        {
            return Task.Run(() =>
            {
                ClearErrorFlags();
                try
                {
                    if (source == null) return Result;
                    this._Collection.AddRange(source);
                }
                catch (Exception ex)
                {
                    CatchException(ex);
                }
                return Result;
            });
        }
        private Task<ErrorResult> Remove(IEnumerable<TicketInfo> source)
        {
            return Task.Run(() =>
            {
                ClearErrorFlags();
                try
                {
                    source.ToList().ForEach(x => this._Collection.Remove(x));
                }
                catch (Exception ex)
                {
                    CatchException(ex);
                }
                return Result;
            });
        }

        public Task<ErrorResult> FilterDataSetToUI()
        {
            return Task.Run(() =>
            {
                ClearErrorFlags();
                try
                {
                    this.Clear();
                    var Task1 = this.FilterCfg.UpdateFilternGetListtoRemove();
                    var Task2 = this.Add(this.DatasetCfg.GetTicketInfos());
                    Task.WaitAll(Task1, Task2);
                    this.Remove(Task1.Result.Result).Wait();
                    this.UpdateCollection();
                    this.FilterChanged = false;
                }
                catch (Exception ex)
                {
                    CatchException(ex);
                }
                return Result;
            });
        }
    }
}
