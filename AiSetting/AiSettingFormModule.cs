using AiSetting.Views;

namespace AiSetting
{
    public class AiSettingFormModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("AiSettingRegion", typeof(SettingForm));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }
}
