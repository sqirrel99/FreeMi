using FreeMi.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace FreeMi.UI.ViewModels
{
    /// <summary>
    /// Interface for a treeview item view model
    /// </summary>
    public interface ITreeViewItemViewModel
    {
        /// <summary>
        /// Adds the underlying model to the entry collection
        /// </summary>
        /// <param name="models">collection of items</param>
        void AddModel(EntryCollection models);

        /// <summary>
        /// Gets or sets the parent item view model (null at first level)
        /// </summary>
        FolderViewModel Parent { get; set; }

        /// <summary>
        /// Gets the parent items collection
        /// </summary>
        TreeViewItemViewModelCollection ParentCollection { get; }

        /// <summary>
        /// Gets a value indicating wether sub entries can be added or not
        /// </summary>
        bool CanAddSubItems { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the item is selected or not
        /// </summary>
        bool IsSelected { get; set; }

        /// <summary>
        /// Gets or sets the path of the entry
        /// </summary>
        string Path { get; set; }
    }
}
