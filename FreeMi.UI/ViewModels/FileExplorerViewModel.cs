using FreeMi.Core.Entries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeMi.UI.ViewModels
{
    /// <summary>
    /// View model for the file explorer treeview item
    /// </summary>
    public class FileExplorerViewModel : TreeViewItemViewModel<FileExplorer>
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the FileExplorerViewModel class
        /// </summary>
        /// <param name="parentViewModel">parent view model</param>
        /// <param name="fileExplorer">file explorer item</param>
        public FileExplorerViewModel(MainViewModel parentViewModel, FileExplorer fileExplorer)
            : base(parentViewModel, fileExplorer)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the key of the icon to show
        /// </summary>
        public override string IconKey
        {
            get { return "MyComputerIcon"; }
        }

        #endregion
    }
}
