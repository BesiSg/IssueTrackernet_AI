using Handlers;
using System.Collections.ObjectModel;
using System.Windows;
using Utility;
using Utility.Lib.Filter;
using Utility.Lib.SettingHandler;

namespace FilterListModule.ViewModels
{
    public class FilterItemViewModel : BaseUtility
    {
        private SettingHandler<Filters> _settinghandlerfilters;
        public FilterHandler filterHandler { get; private set; }
        public Filters Filters => _settinghandlerfilters.Get;
        public ObservableCollection<FilterItem> filterItems { get; private set; } = new ObservableCollection<FilterItem>();
        public DelegateCommand ClearAllCommand { get; private set; }
        public DelegateCommand CheckAllCommand { get; private set; }
        public FilterItemViewModel(SettingHandler<Filters> filters, FilterHandler filter)
        {
            _settinghandlerfilters = filters;
            filterHandler = filter;
            filterHandler.filtersChanged += filtersChangedEvent;
            ClearAllCommand = new DelegateCommand(ClearAll);
            CheckAllCommand = new DelegateCommand(CheckAll);
        }

        private void filtersChangedEvent(object? sender, List<FilterItem> e)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)delegate // <--- HERE
            {
                filterItems.Clear();
                filterItems.AddRange(e);
            });
        }

        private void ClearAll()
        {
            filterHandler.ClearAll();
        }
        private void CheckAll()
        {
            filterHandler.CheckAll();
        }
    }
}
