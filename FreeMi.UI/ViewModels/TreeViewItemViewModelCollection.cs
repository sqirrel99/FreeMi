using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace FreeMi.UI.ViewModels
{
    /// <summary>
    /// Observable collection of TreeViewItemViewModels
    /// </summary>
    public class TreeViewItemViewModelCollection : ObservableCollection<ITreeViewItemViewModel>
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of TreeViewItemViewModelCollection
        /// </summary>
        /// <param name="parentViewModel">parent item view model (or null at first level)</param>
        public TreeViewItemViewModelCollection(FolderViewModel parentViewModel)
        {
            ParentViewModel = parentViewModel;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the parent item view model (or null at first level)
        /// </summary>
        private FolderViewModel ParentViewModel { get; set; }

        #endregion

        #region Methods       

        /// <summary>
        /// Raises the CollectionChanged event with the provided arguments.
        /// </summary>
        /// <param name="e">Arguments of the event being raised.</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (ITreeViewItemViewModel treeViewItemViewModel in e.NewItems)
                    {
                        treeViewItemViewModel.Parent = ParentViewModel;
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (ITreeViewItemViewModel treeViewItemViewModel in e.OldItems)
                    {
                        treeViewItemViewModel.Parent = null;
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    foreach (ITreeViewItemViewModel treeViewItemViewModel in e.OldItems)
                    {
                        treeViewItemViewModel.Parent = null;
                    }
                    foreach (ITreeViewItemViewModel treeViewItemViewModel in e.NewItems)
                    {
                        treeViewItemViewModel.Parent = ParentViewModel;
                    }
                    break;
            }
        }

        #endregion
    }
}
