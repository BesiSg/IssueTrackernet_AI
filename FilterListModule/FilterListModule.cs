using FilterListModule.Views;

namespace FilterListModule
{
    public class FilterListModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("FilterRegion", typeof(FilterList));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }

}
