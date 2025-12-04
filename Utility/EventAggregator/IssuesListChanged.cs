using Utility.Lib.Ticket;

namespace Utility.EventAggregator
{
    public class IssuesListChanged : PubSubEvent<List<TicketInfo>>
    {
    }
}
