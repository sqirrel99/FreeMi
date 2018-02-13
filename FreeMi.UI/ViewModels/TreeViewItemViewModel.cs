using FreeMi.Core;
using FreeMi.Core.Entries;
using FreeMi.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace FreeMi.UI.ViewModels
{
    /// <summary>
    /// Base class for the treeview item view models
    /// </summary>
    /// <typeparam name="TEntry">model type</typeparam>
    public abstract class TreeViewItemViewModel<TEntry> : ViewModelBase, ITreeViewItemViewModel where TEntry : Entry
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the TreeViewItemViewModel class
        /// </summary>
        /// <param name="parentViewModel">parent view model</param>
        /// <param name="model">model</param>
        public TreeViewItemViewModel(MainViewModel parentViewModel, TEntry model)
        {
            ParentViewModel = parentViewModel;
            Model = model;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the main view model
        /// </summary>
        public MainViewModel ParentViewModel { get; private set; }

        /// <summary>
        /// Gets the key of the icon to show
        /// </summary>
        public virtual string IconKey { get { return null; } }

        /// <summary>
        /// Gets or sets the path of the entry
        /// </summary>
        public string Path
        {
            get { return Model.Path; }
            set
            {
                if (Path != value)
                {
                    Model.Path = value;
                    RaisePropertyChanged(() => Path);
                    RaisePropertyChanged(() => Label);
                }
            }
        }

        /// <summary>
        /// Gets or sets the label of the entry
        /// </summary>
        public string Text
        {
            get { return Model.Label; }
            set
            {
                if (Text != value)
                {
                    Model.Label = value;
                    RaisePropertyChanged(() => Text);
                    RaisePropertyChanged(() => Label);
                }
            }
        }

        /// <summary>
        /// Gets the label of the treeview item
        /// </summary>
        public string Label { get { return String.Format("{0}{1}", Text, String.IsNullOrWhiteSpace(Path) ? null : String.Format(" ({0})", Path)); } }

        /// <summary>
        /// Gets the model
        /// </summary>
        protected TEntry Model { get; private set; }

        private bool _isSelected;
        /// <summary>
        /// Gets or sets a value indicating whether the item is selected or not
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    RaisePropertyChanged(() => IsSelected);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating wether sub entries can be added or not
        /// </summary>
        public virtual bool CanAddSubItems { get { return false; } }

        /// <summary>
        /// Gets a value indicating wether the delete menu must be shown or not
        /// </summary>
        public bool CanDelete { get { return true; } }

        /// <summary>
        /// Gets the parent items collection
        /// </summary>
        public TreeViewItemViewModelCollection ParentCollection
        {
            get { return (Parent == null ? ParentViewModel.Entries : Parent.Children); }
        }

        /// <summary>
        /// Gets the parent item view model (or null at first level)
        /// </summary>
        public FolderViewModel Parent { get; private set; }

        FolderViewModel ITreeViewItemViewModel.Parent
        {
            get { return Parent; }
            set { Parent = value; }
        }        

        #endregion

        #region Methods

        /// <summary>
        /// Adds the underlying model to the entry collection
        /// </summary>
        /// <param name="models">collection of items</param>
        public virtual void AddModel(EntryCollection models)
        {
            models.Add(Model);
        }        

        #endregion
    }
}
