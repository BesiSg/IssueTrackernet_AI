namespace Utility.Lib.ColumnDisplay
{
    public class ColumnDisplay : BaseUtility
    {
        public NameBoolPair vKey { get; set; } = new NameBoolPair() { Name = "vKey" };
        public NameBoolPair vTSGSD { get; set; } = new NameBoolPair() { Name = "vTSGSD" };
        public NameBoolPair vSummary { get; set; } = new NameBoolPair() { Name = "vSummary" };
        public NameBoolPair vAssignee { get; set; } = new NameBoolPair() { Name = "vAssignee" };
        public NameBoolPair vStatus { get; set; } = new NameBoolPair() { Name = "vStatus" };
        public NameBoolPair vResolution { get; set; } = new NameBoolPair() { Name = "vResolution" };
        public NameBoolPair vCreated { get; set; } = new NameBoolPair() { Name = "vCreated" };
        public NameBoolPair vUpdated { get; set; } = new NameBoolPair() { Name = "vUpdated" };
        public NameBoolPair vPkgDepartment { get; set; } = new NameBoolPair() { Name = "vPkgDepartment" };
        public NameBoolPair vSite { get; set; } = new NameBoolPair() { Name = "vSite" };
        public NameBoolPair vMachineCustomer { get; set; } = new NameBoolPair() { Name = "vMachineCustomer" };
        public NameBoolPair vMachineProductLine { get; set; } = new NameBoolPair() { Name = "vMachineProductLine" };
        public NameBoolPair vRemark { get; set; } = new NameBoolPair() { Name = "vRemark" };
        public NameBoolPair vDifficulty { get; set; } = new NameBoolPair() { Name = "vDifficulty" };
        public NameBoolPair vHandledBy { get; set; } = new NameBoolPair() { Name = "vHandledBy" };
        public NameBoolPair vNextAction { get; set; } = new NameBoolPair() { Name = "vNextAction" };
        public NameBoolPair vInvestigation { get; set; } = new NameBoolPair() { Name = "vInvestigation" };
        public NameBoolPair vProject { get; set; } = new NameBoolPair() { Name = "vProject" };
        public NameBoolPair vHotPriority { get; set; } = new NameBoolPair() { Name = "vHotPriority" };
        public NameBoolPair vComments { get; set; } = new NameBoolPair() { Name = "vComments" };
        public NameBoolPair vSummarizedStatus { get; set; } = new NameBoolPair() { Name = "vSummarizedStatus" };
        
        public void Copy(ColumnDisplay source)
        {
            var Properties = source.GetType().GetProperties();
            foreach (var property in Properties)
            {
                (property.GetValue(this) as NameBoolPair).Copy(property.GetValue(source) as NameBoolPair);
            }
        }
    }
}
