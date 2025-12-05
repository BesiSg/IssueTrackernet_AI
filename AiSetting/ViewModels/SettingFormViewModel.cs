using Utility;
using Utility.Lib.SettingHandler;
using Utility.Lib.Ticket;

namespace AiSetting.ViewModels
{
    public class SettingFormViewModel : BaseUtility
    {
        private SettingHandler<AIDataset> aiSettingHandler;
        public AIDataset aiDataset => aiSettingHandler.Get;
        public SettingFormViewModel(SettingHandler<AIDataset> setting)
        {
            aiSettingHandler = setting;
        }
    }
}
