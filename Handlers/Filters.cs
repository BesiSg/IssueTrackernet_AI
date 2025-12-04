using System.Linq.Expressions;
using Utility;
using Utility.Lib.Filter;
using Utility.Lib.Ticket;

namespace Handlers
{
    [Serializable]
    public class Filters : aSaveable<FilterHandler>
    {
        private Dataset Dataset;
        public event EventHandler<FilterItem> SelectionChange;
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
                        var distinct = this.Dataset.GetValues().Select(lambda.Compile())?.Distinct();
                        this[filt].Add(distinct);
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
                    foreach (var filter in this.Data)
                    {
                        foreach (var fil in filter.Value.GetDisabled())
                        {
                            var x = Expression.Parameter(typeof(TicketInfo), "x");
                            var body = Expression.PropertyOrField(x, filter.Key);
                            var call = Expression.Equal(body, Expression.Constant(fil.Entry));
                            var lambda = Expression.Lambda<Func<TicketInfo, bool>>(call, x);
                            this.Dataset.GetValues().Where(lambda.Compile()).ToList().ForEach(ticket => Lst[ticket.Key.Value] = ticket);
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
                    Lst = this.GetListToRemove().Result.Result;
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
            this.SelectionChange?.Invoke(this, null);
        }

        public FilterHandler GetHandlerByName(string Name)
        {
            return this.GetType()?.GetProperty(Name)?.GetValue(this) as FilterHandler;
        }
        public FilterHandler GetFilterHandlerByName(string Name)
        {
            if (!ContainsKey(Name))
                this[Name] = new FilterHandler(Name);
            return this[Name];
        }
        public IEnumerable<FilterItem> GetCollectionByName(string Name)
        {
            return this[Name]?._Collection;
        }
        private IEnumerable<FilterItem> GetAllCollection()
        {
            var Col = new List<FilterItem>();
            this.GetValues().ToList().ForEach(property => Col.AddRange(property._Collection));
            return Col;
        }
        public Filters()
        {
            foreach (var filt in Enum.GetNames(typeof(eFilter)))
            {
                if (ContainsKey(filt) == true) continue;
                this[filt] = new FilterHandler(filt);
            }
        }
    }
}
