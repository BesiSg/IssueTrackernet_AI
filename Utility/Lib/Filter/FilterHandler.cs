using System.Xml.Serialization;

namespace Utility.Lib.Filter
{
    [Serializable]
    public class FilterHandler : BaseUtility
    {
        [XmlIgnore]
        public EventHandler<List<FilterItem>>? filtersChanged;
        public FilterHandler() : this(string.Empty)
        {
        }
        public FilterHandler(string name)
        {
            LabelName = name;
        }
        public string LabelName
        {
            get => GetValue(() => LabelName);
            set => SetValue(() => LabelName, value);
        }
        public List<FilterItem> _Collection { get; set; } = new List<FilterItem>();
        private void Add(object item, bool Selected = true)
        {
            if (_Collection.Any(x => x.Entry?.ToString() == item?.ToString())) return;
            _Collection.Add(new FilterItem(item, Selected));
        }
        public void Add(IEnumerable<object> source)
        {
            if (source == null) return;
            lock (_Collection)
            {
                source.ToList().ForEach(x => Add(x));
                _Collection = _Collection.OrderBy(x => x.Entry?.ToString()).ToList();
            }
            filtersChanged?.Invoke(this, _Collection);
        }
        public IEnumerable<FilterItem> GetDisabled()
        {
            return _Collection.Where(x => x.Selected == false);
        }
        public void ClearAll()
        {
            this._Collection.ForEach(x => x.Selected = false);
        }
        public void CheckAll()
        {
            this._Collection.ForEach(x => x.Selected = true);
        }
    }
}
