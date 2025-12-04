using Utility.Lib.ColumnDisplay;

namespace Utility.Lib.PathConfig
{
    [Serializable]
    public class PathConfig() : BaseUtility
    {
        public ColumnDisplay.ColumnDisplay ColDisplay { get; set; } = new ColumnDisplay.ColumnDisplay();
        public IEnumerable<NameBoolPair> GetVisList()
        {
            var _ColDisplay = new List<NameBoolPair>();
            var properties = typeof(ColumnDisplay.ColumnDisplay).GetProperties();
            foreach (var property in properties)
            {
                _ColDisplay.Add(property.GetValue(ColDisplay) as NameBoolPair);
            }
            return _ColDisplay;
        }
    }
}
