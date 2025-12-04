using Handlers;
using Utility;
using Utility.Lib.SettingHandler;

namespace FilterListModule.ViewModels
{
    public class FilterListViewModel : BaseUtility
    {
        public SettingHandler<Filters> _settinghandlerfilters { get; private set; }
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
