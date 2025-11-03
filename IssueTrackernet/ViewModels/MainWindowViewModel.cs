using IssuelistModule.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using static Microsoft.IO.RecyclableMemoryStreamManager;

namespace IssueTrackernet.ViewModels
{
    public class MainWindowViewModel : BaseUtility
    {
        public DelegateCommand SaveDataCommand { get; private set; }
        public DelegateCommand ImportDataCommand { get; private set; }
        public DelegateCommand OnClosingCommand { get; private set; }



        private IssuelistViewModel issueListViewModel;
        public MainWindowViewModel(IssuelistViewModel issuelist)
        {
            this.issueListViewModel = issuelist;
            SaveDataCommand = new DelegateCommand(() => SaveData());
            ImportDataCommand = new DelegateCommand(() => ImportData());
            OnClosingCommand = new DelegateCommand(() => SaveData());
        }
        private void SaveData()
        {
            this.issueListViewModel.Save();
        }
        private void ImportData()
        {
            this.issueListViewModel.DatasetCfg.UpdatefromJira();
        }
    }
}
