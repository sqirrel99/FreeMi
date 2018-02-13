using FreeMi.Core.Entries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeMi.UI.ViewModels
{
    /// <summary>
    /// View model for the podcast treeview item
    /// </summary>
    public class PodcastViewModel : TreeViewItemViewModel<Podcast>
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the PodcastViewModel
        /// </summary>
        /// <param name="parentViewModel">parent view model</param>
        /// <param name="podcast">podcast item</param>
        public PodcastViewModel(MainViewModel parentViewModel, Podcast podcast)
            : base(parentViewModel, podcast)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the key of the icon to show
        /// </summary>
        public override string IconKey
        {
            get { return "RSSFeeds"; }
        }

        #endregion
    }
}
