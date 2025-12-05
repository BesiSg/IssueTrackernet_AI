using BesiAI;
using Handlers.EventAggregator;
using System.Diagnostics;
using System.Windows.Input;
using Utility;
using Utility.Lib.Ticket;

namespace IssueFormModule.ViewModels
{
    public class IssueFormViewModel : BaseUtility
    {
        public DelegateCommand SummarizeCommand { get; private set; }
        public ICommand OpenDetailsCommand { get; private set; }
        private AIHandler AIHandler;
        IEventAggregator _ea;
        public IssueFormViewModel(IEventAggregator ea, AIHandler handler)
        {
            _ea = ea;
            AIHandler = handler;
            _ea.GetEvent<IssueSelectionChange>().Subscribe(SelectedChanged);
            OpenDetailsCommand = new RelayCommand(OpenDetails);
            SummarizeCommand = new DelegateCommand(async () => await Summarize());
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
        private async Task Summarize()
        {
            var task = AIHandler.GetAnswerAsync(Selected.Comments);
            await task.ConfigureAwait(false);
            Selected.SummarizedStatus = task.Result.Item2;
        }

    }
}
