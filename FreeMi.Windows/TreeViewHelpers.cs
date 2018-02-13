using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace FreeMi.Windows
{
    /// <summary>
    /// TreeViewSelectedItem attached property
    /// </summary>
    public static class TreeViewHelpers
    {
        /// <summary>
        /// Gets the selected item
        /// </summary>
        public static object GetTreeViewSelectedItem(DependencyObject obj)
        {
            return (object)obj.GetValue(TreeViewSelectedItemProperty);
        }

        /// <summary>
        /// Sets the selected item
        /// </summary>
        public static void SetTreeViewSelectedItem(DependencyObject obj, object value)
        {
            obj.SetValue(TreeViewSelectedItemProperty, value);
        }

        /// <summary>
        /// TreeViewSelectedItem attached property
        /// </summary>
        public static readonly DependencyProperty TreeViewSelectedItemProperty =
            DependencyProperty.RegisterAttached("TreeViewSelectedItem", typeof(object), typeof(TreeViewHelpers), new PropertyMetadata(new object(), TreeViewSelectedItemChanged));

        private static void TreeViewSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var treeView = sender as TreeView;
            if (treeView == null)
            {
                return;
            }

            treeView.SelectedItemChanged -= treeView_SelectedItemChanged;
            treeView.SelectedItemChanged += treeView_SelectedItemChanged;

            SelectItem(e.NewValue, treeView.ItemContainerGenerator, treeView.Items.Count);
        }

        private static void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SetTreeViewSelectedItem(sender as TreeView, e.NewValue);
        }

        private static bool SelectItem(object o, ItemContainerGenerator itemContainerGenerator, int itemsCount)
        {
            if (itemContainerGenerator == null)
            {
                return false;
            }

            var item = itemContainerGenerator.ContainerFromItem(o) as TreeViewItem;
            if (item != null)
            {
                item.IsSelected = true;
                return true;
            }

            for (int i = 0; i < itemsCount; i++)
            {
                var treeViewItem = itemContainerGenerator.ContainerFromIndex(i) as TreeViewItem;
                if (treeViewItem != null && SelectItem(o, treeViewItem.ItemContainerGenerator, treeViewItem.Items.Count))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
