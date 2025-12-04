using System.Collections.ObjectModel;
using Utility;
using Utility.Lib.ColumnDisplay;
using Utility.Lib.PathConfig;
using Utility.Lib.SettingHandler;

namespace IssuelistModule.ViewModels
{
    public class ColDisplaySetViewModel : BaseUtility
    {
        IEventAggregator _ea;
        public ObservableCollection<NameBoolPair> Data { get; private set; } = new ObservableCollection<NameBoolPair>();
        private SettingHandler<PathConfig> pathcfgHandler;
        private PathConfig _pathconfig => pathcfgHandler.Get;
        public ColDisplaySetViewModel(IEventAggregator ea, SettingHandler<PathConfig> pathcfg)
        {
            _ea = ea;
            pathcfgHandler = pathcfg;
            UpdateVisList(_pathconfig.GetVisList());
        }
        private void UpdateVisList(IEnumerable<NameBoolPair> source)
        {
            Data.Clear();
            Data.AddRange(source);
        }
    }
}
