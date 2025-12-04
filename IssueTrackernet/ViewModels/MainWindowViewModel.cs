using Handlers;
using Utility;
using Utility.EventAggregator;
using Utility.Lib.PathConfig;
using Utility.Lib.SettingHandler;
using Utility.Lib.Ticket;

namespace IssueTrackernet.ViewModels
{
    public class MainWindowViewModel : BaseUtility
    {
        private SettingHandler<PathConfig> pathcfgHandler;
        private SettingHandler<Filters> filtercfgHandler;
        private SettingHandler<Dataset> datasetcfgHandler;
        private SettingHandler<AIDataset> aidatasetHandler;
        private DataSetHandler datasetHandler;
        static string StartupPath = AppDomain.CurrentDomain.BaseDirectory;
        const string PathConfig_Filename = "Data\\Path.xml";
        const string FilterConfig_Filename = "Data\\Filter.xml";
        const string Dataset_Filename = "Data\\Dataset.xml";
        const string AiConfig_Filename = "Data\\Ai.xml";
        string PathConfig_Path = $"{StartupPath}{PathConfig_Filename}";
        string FilterConfig_Path = $"{StartupPath}{FilterConfig_Filename}";
        string Dataset_Path = $"{StartupPath}{Dataset_Filename}";
        string AiConfig_Path = $"{StartupPath}{AiConfig_Filename}";
        public Filters FilterCfg => this.filtercfgHandler.Get;
        public PathConfig PathCfg => this.pathcfgHandler.Get;
        private Dataset DatasetCfg => datasetcfgHandler.Get;
        public DelegateCommand SaveDataCommand { get; private set; }
        public DelegateCommand ImportDataCommand { get; private set; }
        public DelegateCommand OnClosingCommand { get; private set; }
        private IEventAggregator _ea;
        public MainWindowViewModel(IEventAggregator ea, SettingHandler<PathConfig> pathcfg, SettingHandler<Filters> filtercfg, SettingHandler<Dataset> datasetcfg, SettingHandler<AIDataset> aiDatasetHandler, DataSetHandler datasethandler)
        {
            _ea = ea;
            pathcfgHandler = pathcfg;
            filtercfgHandler = filtercfg;
            datasetcfgHandler = datasetcfg;
            aidatasetHandler = aiDatasetHandler;
            pathcfgHandler.SetPathnLoad(PathConfig_Path);
            filtercfgHandler.SetPathnLoad(FilterConfig_Path);
            datasetcfgHandler.SetPathnLoad(Dataset_Path);
            aidatasetHandler.SetPathnLoad(AiConfig_Path);
            FilterCfg.SetDataset(DatasetCfg);
            filtercfgHandler.SettingLoaded += FiltercfgHandler_SettingLoaded;
            datasetcfgHandler.SettingLoaded += DatasetcfgHandler_SettingLoaded;
            datasetHandler = datasethandler;
            datasetHandler.SetData(DatasetCfg);
            SaveDataCommand = new DelegateCommand(() => Save());
            ImportDataCommand = new DelegateCommand(() => ImportData());
            OnClosingCommand = new DelegateCommand(() => Save());
        }

        private void DatasetcfgHandler_SettingLoaded(object? sender, EventArgs e)
        {
            FilterCfg.SetDataset(DatasetCfg);
        }

        private void FiltercfgHandler_SettingLoaded(object? sender, EventArgs e)
        {
            FilterCfg.SetDataset(DatasetCfg);
        }

        public void Save()
        {
            this.pathcfgHandler.Save();
            this.filtercfgHandler.Save();
            this.datasetcfgHandler.Save();
            this.aidatasetHandler.Save();
        }
        private async void ImportData()
        {
            await datasetHandler.UpdatefromJira();
            _ea.GetEvent<IssuesListChanged>().Publish(DatasetCfg.GetValues());
        }
    }
}
