using FilterListModule.ViewModels;
using Handlers;
using System.Windows;
using System.Windows.Controls;
using Utility.Lib.Filter;

namespace FilterListModule.Views
{
    /// <summary>
    /// Interaction logic for IssueList.xaml
    /// </summary>
    public partial class FilterList : UserControl
    {
        public FilterList()
        {
            InitializeComponent();
            this.DataContextChanged += FilterList_DataContextChanged;
            this.BindFilters();
        }

        private void FilterList_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            BindFilters();
        }

        private void BindFilters()
        {
            this.Filteritemcontainer.Children.Clear();
            var count = (this.DataContext as FilterListViewModel).Filters.Filter.Count;
            var Gaps = (count * 10) + 20;
            var AllowedWidthforFilters = this.Width - Gaps;
            var Widthperfilter = AllowedWidthforFilters / count;
            foreach (var filter in (this.DataContext as FilterListViewModel).Filters.Filter)
            {
                var usercontrol = new FilterItem();
                usercontrol.Margin = new Thickness(5, 0, 5, 0);
                usercontrol.Name = filter.Key;
                usercontrol.Width = Widthperfilter;
                usercontrol.DataContext = filter.Value;
                this.Filteritemcontainer.Children.Add(usercontrol);
            }
        }
    }
}
