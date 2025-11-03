namespace Utility
{
    //   public enum Resolution
    //   {
    //	[TextAttribute("Unresolved")]
    //	Unresolved,
    //	[TextAttribute("Cancelled")]
    //	Cancelled,
    //	[TextAttribute("Cannot Reproduce")]
    //	CannotReproduce,
    //	[TextAttribute("Declined")]
    //	Declined,
    //	[TextAttribute("Done")]
    //	Done,
    //	[TextAttribute("Fixed")]
    //	Fixed,
    //	[TextAttribute("Hardware Failure")]
    //	HardwareFailure,
    //	[TextAttribute("Incomplete")]
    //	Incomplete,
    //	[TextAttribute("Patched")]
    //	Patched,
    //	[TextAttribute("Ready")]
    //	Ready,
    //	[TextAttribute("Rejected")]
    //	Rejected,
    //	[TextAttribute("Won't Do")]
    //	WontDo,
    //	[TextAttribute("Won't Fix")]
    //	WontFix,
    //}

    public class TextAttribute : Attribute
    {
        public string Attribute { get; }
        public bool UpdatableFromJira { get; }
        public bool Updatable { get; }

        public TextAttribute(string attr, bool UpdatableFromJira, bool Update)
        {
            this.Attribute = attr;
            this.UpdatableFromJira = UpdatableFromJira;
            this.Updatable = Update;
        }
    }
}
