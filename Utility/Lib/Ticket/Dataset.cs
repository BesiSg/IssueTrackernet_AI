namespace Utility.Lib.Ticket
{
    [Serializable]
    public class Dataset : aSaveable
    {
        public string PriorityList
        {
            get => this.GetValue(() => this.PriorityList);
            set => this.SetValue(() => this.PriorityList, value);
        }
        public string PersonalAccessToken
        {
            get => this.GetValue(() => this.PersonalAccessToken);
            set => this.SetValue(() => this.PersonalAccessToken, value);
        }
        public string BesiAIToken
        {
            get => this.GetValue(() => this.BesiAIToken);
            set => this.SetValue(() => this.BesiAIToken, value);
        }
        public string AIQuery
        {
            get => this.GetValue(() => this.AIQuery);
            set => this.SetValue(() => this.AIQuery, value);
        }
        public DateTime LastUpdateDate
        {
            get => this.GetValue(() => this.LastUpdateDate);
            set => this.SetValue(() => this.LastUpdateDate, value);
        }
        public XmlDictionary<string, TicketInfo> Issues { get; set; } = new XmlDictionary<string, TicketInfo>();
        public IEnumerable<TicketInfo> GetTicketInfos()
        {
            return this.Issues?.Values;
        }
    }
}
