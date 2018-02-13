using FreeMi.Core.Entries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeMi.UI.ViewModels
{
    /// <summary>
    /// View model for the file treeview item
    /// </summary>
    public class FileViewModel : TreeViewItemViewModel<File>
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the FileViewModel class
        /// </summary>
        /// <param name="parentViewModel">parent view model</param>
        /// <param name="file">File item</param>
        public FileViewModel(MainViewModel parentViewModel, File file)
            : base(parentViewModel, file)
        {
        }

        #endregion

        #region

        /// <summary>
        /// Gets the key of the icon to show
        /// </summary>
        public override string IconKey { get { return "MovFormat"; } }

        #endregion
    }
}
