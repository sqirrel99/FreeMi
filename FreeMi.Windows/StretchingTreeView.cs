using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace FreeMi.Windows
{
    /// <summary>
    /// StretchingTreeView (http://blogs.msdn.com/b/jpricket/archive/2008/08/05/wpf-a-stretching-treeview.aspx)
    /// </summary>
    public class StretchingTreeView : TreeView
    {
        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new StretchingTreeViewItem();
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>true if the item is (or is eligible to be) its own container; otherwise, false.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is StretchingTreeViewItem;
        }
    }

    /// <summary>
    /// StretchingTreeViewItem
    /// </summary>
    public class StretchingTreeViewItem : TreeViewItem
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of StretchingTreeViewItem class
        /// </summary>
        public StretchingTreeViewItem()
        {
            Loaded += new RoutedEventHandler(StretchingTreeViewItem_Loaded);
        }

        #endregion

        #region Methods

        private void StretchingTreeViewItem_Loaded(object sender, RoutedEventArgs e)
        {
            // The purpose of this code is to stretch the Header Content all the way accross the TreeView. 
            if (VisualChildrenCount > 0)
            {
                var grid = GetVisualChild(0) as Grid;
                if (grid != null && grid.ColumnDefinitions.Count == 3)
                {
                    // Remove the middle column which is set to Auto and let it get replaced with the 
                    // last column that is set to Star.
                    grid.ColumnDefinitions.RemoveAt(1);
                }
            }
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new StretchingTreeViewItem();
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>true if the item is (or is eligible to be) its own container; otherwise, false.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is StretchingTreeViewItem;
        }

        #endregion
    }
}
