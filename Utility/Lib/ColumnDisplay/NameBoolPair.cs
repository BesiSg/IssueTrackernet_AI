namespace Utility.Lib.ColumnDisplay
{
    public class NameBoolPair : BaseUtility
    {
        public string Name
        {
            get => GetValue(() => Name);
            set => SetValue(() => Name, value);
        }
        public bool Display
        {
            get => GetValue(() => Display);
            set => SetValue(() => Display, value);
        }
        public NameBoolPair()
        {
        }
        public void Copy(NameBoolPair source)
        {
            Name = source.Name;
            Display = source.Display;
        }
    }
}
