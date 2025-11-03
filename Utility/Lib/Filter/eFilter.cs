namespace Utility.Lib.Filter
{
    public enum eFilter
    {
        [Text("Assignee", true, true)]
        Assignee,
        [Text("Status", true, true)]
        Status,
        [Text("Resolution", true, true)]
        Resolution,
        [Text("PkgDepartment", true, true)]
        PkgDepartment,
        [Text("Site", true, true)]
        Site,
        [Text("MachineCustomer", true, true)]
        MachineCustomer,
        [Text("MachineProductLine", true, true)]
        MachineProductLine,
        [Text("Difficulty", false, false)]
        Difficulty,
        [Text("HandledBy", false, false)]
        HandledBy,
        [Text("NextAction", false, false)]
        NextAction,
        [Text("Project", true, true)]
        Project,
        [Text("HotPriority", false, false)]
        HotPriority,
    }
}
