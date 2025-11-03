using System.Windows.Controls;
using Utility.Lib.Ticket;

namespace IssuelistModule.Views
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class Issuelist : UserControl
    {
        public Issuelist()
        {
            InitializeComponent();
            this.DiffCB.ItemsSource = Enum.GetNames(typeof(Difficulty));
            this.HandledCB.ItemsSource = Enum.GetNames(typeof(HandledBy));
            this.NextActionCB.ItemsSource = Enum.GetNames(typeof(NextAction));
        }
    }

}
