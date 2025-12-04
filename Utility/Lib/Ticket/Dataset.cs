namespace Utility.Lib.Ticket
{
    [Serializable]
    public class Dataset : BaseUtility
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
        public DateTime LastUpdateDate
        {
            get => this.GetValue(() => this.LastUpdateDate);
            set => this.SetValue(() => this.LastUpdateDate, value);
        }

        public XmlDictionary<string, TicketInfo> Data { get; set; } = new XmlDictionary<string, TicketInfo>();
        public TicketInfo Get(string key)
        {
            if (!ContainsKey(key))
                this[key] = new TicketInfo();
            return this[key];
        }
        public List<string> GetKeys()
        {
            return Data.Keys.ToList();
        }
        public List<TicketInfo> GetValues()
        {
            return Data.Values.ToList();
        }
        public TicketInfo this[string key]
        {
            get
            {
                // Following trick can reduce the range check by one
                if (!ContainsKey(key))
                {
                    throw new KeyNotFoundException(key);
                }
                return Data[key];
            }
            set
            {
                Data[key] = value;
            }
        }
        public bool ContainsKey(string key)
        {
            return Data.ContainsKey(key);
        }
        public void Clear()
        {
            Data.Clear();
        }


    }
}
