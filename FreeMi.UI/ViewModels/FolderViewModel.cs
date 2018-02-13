using FreeMi.Core;
using FreeMi.Core.Entries;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace FreeMi.UI.ViewModels
{
    /// <summary>
    /// ViewModel for the folder treeview item
    /// </summary>
    public class FolderViewModel : TreeViewItemViewModel<Folder>
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the FolderViewModel class
        /// </summary>
        /// <param name="mainViewModel">main view model</param>
        /// <param name="folder">folder</param>
        public FolderViewModel(MainViewModel mainViewModel, Folder folder)
            : base(mainViewModel, folder)
        {
            var children = new TreeViewItemViewModelCollection(this);
            Children = children;
            ITreeViewItemViewModel itemViewModel;
            TreeViewItemViewModelFactory factory = null;
            foreach (var child in folder.Children)
            {
                if (factory == null)
                {
                    factory = new TreeViewItemViewModelFactory();
                }
                itemViewModel = factory.CreateTreeViewItemViewModel(mainViewModel, child);
                children.Add(itemViewModel);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the subitems
        /// </summary>
        public TreeViewItemViewModelCollection Children
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the key of the icon to show
        /// </summary>
        public override string IconKey { get { return "FolderIcon"; } }

        /// <summary>
        /// Gets a value indicating wether sub entries can be added or not
        /// </summary>
        public override bool CanAddSubItems { get { return true; } }

        /// <summary>
        /// Gets or sets the media kind for this folder
        /// </summary>
        public MediaKind MediaKind
        {
            get { return Model.MediaKind; }
            set
            {
                if (MediaKind != value)
                {
                    Model.MediaKind = value;
                    RaisePropertyChanged(() => MediaKind);
                    RaisePropertyChanged(() => IsVideoMediaKind);
                    RaisePropertyChanged(() => IsAudioMediaKind);
                    RaisePropertyChanged(() => IsImageMediaKind);
                    RaisePropertyChanged(() => IsOtherMediaKind);
                }
            }
        }

        private bool _isExpanded;
        /// <summary>
        /// Gets or sets a value indicating wether the node is expanded or not
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    RaisePropertyChanged(() => IsExpanded);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating wether the folder must show the videos or not
        /// </summary>
        public bool IsVideoMediaKind
        {
            get { return (MediaKind & MediaKind.Video) > 0; }
            set { MediaKind = value ? (MediaKind | MediaKind.Video) : (MediaKind & ~MediaKind.Video); }
        }

        /// <summary>
        /// Gets or sets a value indicating wether the folder must show the audio files or not
        /// </summary>
        public bool IsAudioMediaKind
        {
            get { return (MediaKind & MediaKind.Audio) > 0; }
            set { MediaKind = value ? (MediaKind | MediaKind.Audio) : (MediaKind & ~MediaKind.Audio); }
        }

        /// <summary>
        /// Gets or sets a value indicating wether the folder must show the images or not
        /// </summary>
        public bool IsImageMediaKind
        {
            get { return (MediaKind & MediaKind.Image) > 0; }
            set { MediaKind = value ? (MediaKind | MediaKind.Image) : (MediaKind & ~MediaKind.Image); }
        }

        /// <summary>
        /// Gets or sets a value indicating wether the folder must show the other files or not
        /// </summary>
        public bool IsOtherMediaKind
        {
            get { return (MediaKind & MediaKind.Other) > 0; }
            set { MediaKind = value ? (MediaKind | MediaKind.Other) : (MediaKind & ~MediaKind.Other); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the underlying model to the entry collection
        /// </summary>
        /// <param name="models">collection of items</param>
        public override void AddModel(EntryCollection models)
        {
            base.AddModel(models);
            Model.Children.Clear();
            foreach (var subItem in Children)
            {
                subItem.AddModel(Model.Children);
            }
        }

        #endregion
    }
}
