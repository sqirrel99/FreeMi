using FreeMi.Core.Entries;
using FreeMi.Core.Properties;
using FreeMi.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace FreeMi.Windows
{
    /// <summary>
    /// Main view model
    /// </summary>
    class MainViewModel : FreeMi.UI.ViewModels.MainViewModel
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of MainViewModel class
        /// </summary>
        public MainViewModel()
            : base()
        {
            UndoCommand = new RelayCommand(Reload);
            SaveCommand = new RelayCommand(Save);
            NewCommand = new RelayCommand<string>(New);
            RemoveCommand = new RelayCommand(Remove);
            BrowseCommand = new RelayCommand(Browse);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Old Win32Window
        /// </summary>
        public IWin32Window Win32Window { get; set; }

        /// <summary>
        /// Gets the main view model
        /// </summary>
        public MainViewModel ParentViewModel { get { return this; } }

        /// <summary>
        /// Gets the undo command
        /// </summary>
        public ICommand UndoCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the save command
        /// </summary>
        public ICommand SaveCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the command for the new items
        /// </summary>
        public ICommand NewCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the remove command
        /// </summary>
        public ICommand RemoveCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the browse command
        /// </summary>
        public ICommand BrowseCommand
        {
            get;
            private set;
        }
       
        #endregion

        #region Methods

        /// <summary>
        /// Folder browser
        /// </summary>
        public void Browse()
        {
            var selectedItem = SelectedItem;
            if (selectedItem != null)
            {
                using (var folderBrowserDialog = new FolderBrowserDialog())
                {
                    folderBrowserDialog.SelectedPath = selectedItem.Path;
                    if (folderBrowserDialog.ShowDialog(Win32Window) == DialogResult.OK)
                    {
                        selectedItem.Path = folderBrowserDialog.SelectedPath;
                    }
                }
            }
        }

        #endregion
    }
}
