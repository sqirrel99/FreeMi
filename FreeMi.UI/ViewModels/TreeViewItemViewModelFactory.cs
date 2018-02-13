using FreeMi.Core.Entries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeMi.UI.ViewModels
{
    /// <summary>
    /// Factory for the treeview item view models
    /// </summary>
    class TreeViewItemViewModelFactory
    {
        #region Static constructor

        /// <summary>
        /// Fills the view model types collection
        /// </summary>
        static TreeViewItemViewModelFactory()
        {
            var viewModelTypes = new Dictionary<Type, Type>();
            viewModelTypes.Add(typeof(FileExplorer), typeof(FileExplorerViewModel));
            viewModelTypes.Add(typeof(File), typeof(FileViewModel));
            viewModelTypes.Add(typeof(Folder), typeof(FolderViewModel));
            viewModelTypes.Add(typeof(Podcast), typeof(PodcastViewModel));
            viewModelTypes.Add(typeof(Shutdown), typeof(ShutdownViewModel));
            ViewModelTypes = viewModelTypes;
        }

        #endregion

        #region Properties

        private static IDictionary<Type, Type> ViewModelTypes { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// TreeView item view model creation
        /// </summary>
        /// <param name="mainViewModel">main view model</param>
        /// <param name="model">model</param>
        /// <returns>a treeview item view model</returns>
        public ITreeViewItemViewModel CreateTreeViewItemViewModel(MainViewModel mainViewModel, Entry model)
        {
            return (ITreeViewItemViewModel)Activator.CreateInstance(ViewModelTypes[model.GetType()], mainViewModel, model);
        }

        #endregion
    }
}


