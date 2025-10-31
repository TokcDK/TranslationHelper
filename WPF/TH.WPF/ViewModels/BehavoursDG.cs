using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Linq;
using Microsoft.Xaml.Behaviors;

namespace TH.WPF.ViewModels
{
    // https://social.msdn.microsoft.com/Forums/en-US/5041c158-e48b-4efd-9f73-848a85abba83/datagrid-selecteditems-in-mvvm?forum=wpf with tweaks
    public class SelectedItemsBehavior : Behavior<MultiSelector>
    {
        protected override void OnAttached()
        {
            AssociatedObject.SelectionChanged += AssociatedObjectSelectionChanged;
        }
        protected override void OnDetaching()
        {
            AssociatedObject.SelectionChanged -= AssociatedObjectSelectionChanged;
        }

        void AssociatedObjectSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Reuse existing list if possible to avoid unnecessary allocations
            if (SelectedItems == null)
            {
                SelectedItems = new List<object>(AssociatedObject.SelectedItems.Count);
            }
            else
            {
                SelectedItems.Clear();
            }
            
            foreach (var item in AssociatedObject.SelectedItems)
            {
                SelectedItems.Add(item);
            }
        }
        public IList<object> SelectedItems
        {
            get { return (IList<object>)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems"
                , typeof(IList<object>)
                , typeof(SelectedItemsBehavior)
                ,
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    }
}
