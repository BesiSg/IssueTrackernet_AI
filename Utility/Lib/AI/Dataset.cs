namespace Utility.Lib.Ticket
{
    [Serializable]
    public class AIDataset : BaseUtility
    {
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
        public string IPAddress
        {
            get => this.GetValue(() => this.IPAddress);
            set => this.SetValue(() => this.IPAddress, value);
        }
    }

}
