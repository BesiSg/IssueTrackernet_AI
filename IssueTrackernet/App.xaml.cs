using Handlers;
using IssueTrackernet.Views;
using System.Windows;
using Utility.Lib.PathConfig;
using Utility.Lib.SettingHandler;
using Utility.Lib.Ticket;

namespace IssueTrackernet
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        public static MainWindow _MainWindow;

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<SettingHandler<PathConfig>>();
            containerRegistry.RegisterSingleton<SettingHandler<Filters>>();
            containerRegistry.RegisterSingleton<SettingHandler<Dataset>>();
            containerRegistry.RegisterSingleton<SettingHandler<AIDataset>>();
            containerRegistry.RegisterSingleton<DataSetHandler>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<IssuelistModule.IssuelistModule>();
            moduleCatalog.AddModule<FilterListModule.FilterListModule>();
            moduleCatalog.AddModule<IssueFormModule.IssueFormModule>();
        }
    }

}
