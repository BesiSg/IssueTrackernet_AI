using System.Collections.ObjectModel;
using System.Windows;
using System.Xml.Serialization;

namespace Utility.Lib.Filter
{
    [Serializable]
    public class FilterHandler : BaseUtility
    {
        public FilterHandler(string name)
        {
            LabelName = name; 
            ClearAllCommand = new DelegateCommand(ClearAll);
            CheckAllCommand = new DelegateCommand(CheckAll);
        }
        public string LabelName
        {
            get => GetValue(() => this.LabelName);
            set => SetValue(() => this.LabelName, value);
        }
        [XmlIgnore]
        public ObservableCollection<FilterItem> Collection { get; set; } = new ObservableCollection<FilterItem>();
        [XmlIgnore]
        public DelegateCommand ClearAllCommand { get; private set; }
        [XmlIgnore]
        public DelegateCommand CheckAllCommand { get; private set; }
        public FilterHandler():this(string.Empty)
        {
        }

        public List<FilterItem> _Collection { get; set; } = new List<FilterItem>();
        private void Add(object item, bool Selected = true)
        {
            if (this._Collection.Any(x => x.Entry?.ToString() == item?.ToString())) return;
            this._Collection.Add(new FilterItem(item, Selected));
        }
        public void Add(IEnumerable<object> source)
        {
            if (source == null) return;
            source.ToList().ForEach(x => this.Add(x));
            this._Collection = this._Collection.OrderBy(x => x.Entry?.ToString()).ToList();
            Application.Current.Dispatcher.BeginInvoke((Action)delegate // <--- HERE
            {
                this.Collection.Clear();
                this._Collection.ForEach(x => this.Collection.Add(x));
            });
        }
        public IEnumerable<FilterItem> GetDisabled()
        {
            return this._Collection.Where(x => x.Selected == false);
        }
        private void ClearAll()
        {
            this._Collection.ForEach(x => x.Selected = false);
        }
        private void CheckAll()
        {
            this._Collection.ForEach(x => x.Selected = true);
        }
    }
}
