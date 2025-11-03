using System.Diagnostics;
using System.Reflection;
using System.Xml.Serialization;

namespace Utility.Lib.Ticket
{
    public class TicketInfo : BaseUtility
    {
        [TextAttribute("Key", true, false)]
        public sEntry Key
        {
            get => GetValue(() => Key);
            set => SetValue(() => Key, value);
        }
        [TextAttribute("Summary", true, true)]
        public sEntry Summary
        {
            get => GetValue(() => Summary);
            set => SetValue(() => Summary, value);
        }
        [TextAttribute("TSGSD", false, true)]
        public string TSGSD
        {
            get => GetValue(() => TSGSD);
            set => SetValue(() => TSGSD, value);
        }
        [TextAttribute("Assignee", true, true)]
        public string Assignee
        {
            get => GetValue(() => Assignee);
            set => SetValue(() => Assignee, value);
        }
        [TextAttribute("Status", true, true)]
        public string Status
        {
            get => GetValue(() => Status);
            set => SetValue(() => Status, value);
        }
        [TextAttribute("Resolution", true, true)]
        public string Resolution
        {
            get => GetValue(() => Resolution);
            set => SetValue(() => Resolution, value);
        }
        [TextAttribute("Created", true, true)]
        public DateTime Created
        {
            get => GetValue(() => Created);
            set => SetValue(() => Created, value);
        }
        [TextAttribute("Updated", true, true)]
        public DateTime Updated
        {
            get => GetValue(() => Updated);
            set => SetValue(() => Updated, value);
        }
        [TextAttribute("Pkg Department", true, true)]
        public string PkgDepartment
        {
            get => GetValue(() => PkgDepartment);
            set => SetValue(() => PkgDepartment, value);
        }
        [TextAttribute("Site", true, true)]
        public string Site
        {
            get => GetValue(() => Site);
            set => SetValue(() => Site, value);
        }
        [TextAttribute("Machine Customer", true, true)]
        public string MachineCustomer
        {
            get => GetValue(() => MachineCustomer);
            set => SetValue(() => MachineCustomer, value);
        }
        [TextAttribute("Machine Product Line", true, true)]
        public string MachineProductLine
        {
            get => GetValue(() => MachineProductLine);
            set => SetValue(() => MachineProductLine, value);
        }
        [TextAttribute("Remark", false, false)]
        public string Remark
        {
            get => GetValue(() => Remark);
            set => SetValue(() => Remark, value);
        }
        [TextAttribute("Difficulty", false, false)]
        public string Difficulty
        {
            get => GetValue(() => Difficulty);
            set => SetValue(() => Difficulty, value);
        }
        [TextAttribute("HandledBy", false, false)]
        public string HandledBy
        {
            get => GetValue(() => HandledBy);
            set => SetValue(() => HandledBy, value);
        }
        [TextAttribute("NextAction", false, false)]
        public string NextAction
        {
            get => GetValue(() => NextAction);
            set => SetValue(() => NextAction, value);
        }
        [TextAttribute("Investigation", false, false)]
        public string Investigation
        {
            get => GetValue(() => Investigation);
            set => SetValue(() => Investigation, value);
        }
        [TextAttribute("Project", true, true)]
        public string Project
        {
            get => GetValue(() => Project);
            set => SetValue(() => Project, value);
        }
        [TextAttribute("HotPriority", false, false)]
        public string HotPriority
        {
            get => GetValue(() => HotPriority);
            set => SetValue(() => HotPriority, value);
        }
        [TextAttribute("Comments", true, true)]
        public string Comments
        {
            get => GetValue(() => Comments);
            set => SetValue(() => Comments, value);
        }
        [TextAttribute("Remark", false, false)]
        public string SummarizedStatus
        {
            get => GetValue(() => SummarizedStatus);
            set => SetValue(() => SummarizedStatus, value);
        }
        public void Copy(TicketInfo source)
        {
            var Properties = source.GetType().GetProperties();
            foreach (var property in Properties)
            {
                property.SetValue(this, property.GetValue(source));
            }
        }
        public void UpdateStatus(TicketInfo source)
        {
            var Properties = source.GetType().GetProperties();
            foreach (var property in Properties)
            {
                if (property.GetCustomAttribute<TextAttribute>()?.Updatable==true)
                    property.SetValue(this, property.GetValue(source));
            }
        }
        [XmlIgnore]
        public DelegateCommand<Uri> RequestedNavigateCommand { get; private set; }
        public TicketInfo()
        {
            RequestedNavigateCommand = new DelegateCommand<Uri>((selecteduri) =>
            {
                System.Diagnostics.Process.Start(new ProcessStartInfo(selecteduri.AbsoluteUri) { UseShellExecute = true });
            });
        }
    }
    public enum Difficulty
    {
        Unselected,
        Easy,
        Mid,
        Hard,
    }
    public enum HandledBy
    {
        Unselected,
        PCBS,
        FGP,
        SDR,
    }
    public enum NextAction
    {
        LowPriority,
        PendingInfo,
        ActionNeeded,
        Done,
    }
}
