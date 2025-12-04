using IssueFormModule.Views;

namespace IssueFormModule
{
    public class IssueFormModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("IssueFormRegion", typeof(IssueForm));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }

}
