using IssuelistModule.Views;

namespace IssuelistModule
{
    public class IssuelistModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("ContentRegion", typeof(Issuelist));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }

}
