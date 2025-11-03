using Handlers;
using Handlers.EventAggregator;
using Utility;
using Utility.Lib.Filter;
using Utility.Lib.SettingHandler;

namespace FilterListModule.ViewModels
{
    public class FilterListViewModel : BaseUtility
    {
        private SettingHandler<Filters> _settinghandlerfilters;
        public Filters Filters => _settinghandlerfilters.Get;
        public DelegateCommand RefreshListCommand { get; private set; }

        public FilterListViewModel(SettingHandler<Filters> filters)
        {
            _settinghandlerfilters = filters;
            RefreshListCommand = new DelegateCommand(() => refreshlistCommand());
        }
        private void refreshlistCommand()
        {
            this.Filters.UpdateFilters();
        }
    }
}
