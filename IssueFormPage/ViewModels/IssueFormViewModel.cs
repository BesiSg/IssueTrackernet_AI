using Handlers.EventAggregator;
using System.Diagnostics;
using System.Security.Policy;
using System.Windows.Input;
using Utility;
using Utility.Lib.Ticket;

namespace IssueFormModule.ViewModels
{
    public class IssueFormViewModel : BaseUtility
    {
        public DelegateCommand RefreshListCommand { get; private set; }
        public ICommand OpenDetailsCommand { get; private set; }
        IEventAggregator _ea;
        public IssueFormViewModel(IEventAggregator ea)
        {
            _ea = ea;
            _ea.GetEvent<IssueSelectionChange>().Subscribe(SelectedChanged);
            OpenDetailsCommand = new RelayCommand(OpenDetails);
        }

        private void OpenDetails(object obj)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = (string)obj,
                UseShellExecute = true
            });
        }

        public TicketInfo Selected
        {
            get => GetValue(() => Selected);
            set => SetValue(() => Selected, value);
        }
        private void SelectedChanged(TicketInfo selected)
        {
            Selected = selected;
        }


    }
}
