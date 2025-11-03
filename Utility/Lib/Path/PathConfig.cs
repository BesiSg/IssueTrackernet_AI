using System.Xml.Serialization;
using Utility.Lib.ColumnDisplay;

namespace Utility.Lib.PathConfig
{
    [Serializable]
    public class PathConfig : aSaveable
    {
        public event EventHandler SavePathChanged;
        public event EventHandler LoadPathChanged;
        [XmlIgnore]
        public DelegateCommand LoadDataSet { get; private set; }
        [XmlIgnore]
        public DelegateCommand SaveDataSet { get; private set; }
        public string DataPath
        {
            get => this.GetValue(() => DataPath);
            set => this.SetValue(() => DataPath, value);
        }
        public ColumnDisplay.ColumnDisplay ColDisplay { get; set; } = new ColumnDisplay.ColumnDisplay();
        private void SetSavePath(string path)
        {
            DataPath = path;
            SavePathChanged?.Invoke(this, null);
        }
        private void SetLoadPath(string path)
        {
            DataPath = path;
            LoadPathChanged?.Invoke(this, null);
        }
        public PathConfig()
        {
            SaveDataSet = new DelegateCommand(() => _saveDataset());
            LoadDataSet = new DelegateCommand(() => _loadDataset());
        }
        private void _loadDataset()
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            openFileDlg.DefaultExt = ".xml";
            openFileDlg.Filter = "(.xml)|*.xml";
            var result = openFileDlg.ShowDialog();
            if (result == true)
            {
                this.SetLoadPath(openFileDlg.FileName);
            }
        }
        private void _saveDataset()
        {
            Microsoft.Win32.SaveFileDialog saveFileDlg = new Microsoft.Win32.SaveFileDialog();
            saveFileDlg.DefaultExt = ".xml";
            saveFileDlg.Filter = "(.xml)|*.xml";
            var result = saveFileDlg.ShowDialog();
            if (result == true)
            {
                this.SetSavePath(saveFileDlg.FileName);
            }
        }

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
