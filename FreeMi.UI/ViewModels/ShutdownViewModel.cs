using FreeMi.Core.Entries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeMi.UI.ViewModels
{
    /// <summary>
    /// View model for the shutdown treeview item
    /// </summary>
    public class ShutdownViewModel : TreeViewItemViewModel<Shutdown>
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance fo the ShutdownViewModel class
        /// </summary>
        /// <param name="parentViewModel">parent view model</param>
        /// <param name="shutdown">Shutdown item</param>
        public ShutdownViewModel(MainViewModel parentViewModel, Shutdown shutdown)
            : base(parentViewModel, shutdown)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the key of the icon to show
        /// </summary>
        public override string IconKey
        {
            get { return "Power"; }
        }

        #endregion
    }
}
