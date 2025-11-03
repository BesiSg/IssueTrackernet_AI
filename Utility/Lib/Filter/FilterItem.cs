namespace Utility.Lib.Filter
{
    [Serializable]
    public class FilterItem : BaseUtility
    {
        public event EventHandler SelectionChange;
        public object Entry
        {
            get => GetValue(() => Entry);
            set => SetValue(() => Entry, value);
        }
        public bool Selected
        {
            get => GetValue(() => Selected);
            set
            {
                SetValue(() => Selected, value);
                SelectionChange?.Invoke(this, null);
            }
        }
        public FilterItem()
        {
        }
        public FilterItem(object entry, bool selected)
        {
            Entry = entry;
            Selected = selected;
        }
    }
}
