using System.Linq.Expressions;
using Utility;
using Utility.Lib.Filter;
using Utility.Lib.Ticket;

namespace Handlers
{
    [Serializable]
    public class Filters : aSaveable
    {
        private Dataset Dataset;
        public event EventHandler<FilterItem> SelectionChange;
        public XmlDictionary<string, FilterHandler> Filter { get; set; } = new XmlDictionary<string, FilterHandler>();
        public Task<ErrorResult> UpdateFilters()
        {
            return Task.Run(() =>
            {
                ClearErrorFlags();
                try
                {
                    GetAllCollection().ToList().ForEach(x => x.SelectionChange -= Filter_SelectionChange);
                    foreach (var filt in Enum.GetNames(typeof(eFilter)))
                    {
                        var x = Expression.Parameter(typeof(TicketInfo), filt);
                        var body = Expression.PropertyOrField(x, filt);
                        var lambda = Expression.Lambda<Func<TicketInfo, string>>(body, x);
                        var distinct = this.Dataset.GetTicketInfos().Select(lambda.Compile())?.Distinct();
                        this.Filter[filt].Add(distinct);
                    }
                    GetAllCollection().ToList().ForEach(x => x.SelectionChange += Filter_SelectionChange);
                }
                catch (Exception ex)
                {
                    CatchException(ex);
                }
                return Result;
            });
        }
        private Task<(ErrorResult Error, List<TicketInfo> Result)> GetListToRemove()
        {
            return Task.Run(() =>
            {
                var Lst = new Dictionary<string, TicketInfo>();
                ClearErrorFlags();
                try
                {
                    foreach (var filter in this.Filter)
                    {
                        foreach (var fil in filter.Value.GetDisabled())
                        {
                            var x = Expression.Parameter(typeof(TicketInfo), "x");
                            var body = Expression.PropertyOrField(x, filter.Key);
                            var call = Expression.Equal(body, Expression.Constant(fil.Entry));
                            var lambda = Expression.Lambda<Func<TicketInfo, bool>>(call, x);
                            this.Dataset.GetTicketInfos().Where(lambda.Compile()).ToList().ForEach(ticket => Lst[ticket.Key.Value] = ticket);
                            //foreach (var ticket in this.Dataset.GetTicketInfos().Where(lambda.Compile()))
                            //    Lst[ticket.Key.Value] = ticket;
                        }
                    }
                }
                catch (Exception ex)
                {
                    CatchException(ex);
                }
                return (Result, Lst.Values.ToList());
            });
        }
        public Task<(ErrorResult Error, List<TicketInfo> Result)> UpdateFilternGetListtoRemove()
        {
            return Task.Run(() =>
            {
                var Lst = new List<TicketInfo>();
                ClearErrorFlags();
                try
                {
                    this.UpdateFilters().Wait();
                    var Task2 = this.GetListToRemove();
                    Task2.Wait();
                    Lst = Task2.Result.Result;
                }
                catch (Exception ex)
                {
                    CatchException(ex);
                }
                return (Result, Lst);
            });
        }

        public void SetDataset(Dataset source)
        {
            this.Dataset = source;
        }
        private void Filter_SelectionChange(object sender, EventArgs e)
        {
            this.SelectionChange?.Invoke(null, null);
        }

        public FilterHandler GetHandlerByName(string Name)
        {
            return this.GetType()?.GetProperty(Name)?.GetValue(this) as FilterHandler;
        }
        public FilterHandler GetFilterHandlerByName(string Name)
        {
            if (!Filter.ContainsKey(Name))
                Filter[Name] = new FilterHandler(Name);
            return Filter[Name];
        }
        public IEnumerable<FilterItem> GetCollectionByName(string Name)
        {
            return Filter[Name]?._Collection;
        }
        private IEnumerable<FilterItem> GetAllCollection()
        {
            List<FilterItem> Col = new List<FilterItem>();
            this.Filter.Values.ToList().ForEach(property=>Col.AddRange(property._Collection));
            return Col;
        }
        public Filters()
        {
            foreach (var filt in Enum.GetNames(typeof(eFilter)))
            {
                if (this.Filter?.ContainsKey(filt) == true) continue;
                this.Filter[filt] = new FilterHandler(filt);
            }
        }
    }
}
