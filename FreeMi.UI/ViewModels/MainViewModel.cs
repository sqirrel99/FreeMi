using FreeMi.Core;
using FreeMi.Core.Entries;
using FreeMi.Core.Properties;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FreeMi.UI.ViewModels
{
    /// <summary>
    /// Main ViewModel
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of MainViewModel class
        /// </summary>
        public MainViewModel()
        {
            BufferSizesV5 = GetBufferSizes(256);
            BufferSizesV6 = GetBufferSizes(64);
            Init();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether some parameters were changed and saved
        /// </summary>
        public bool IsDirty { get; private set; }

        /// <summary>
        /// Gets the product name
        /// </summary>
        public string ProductName
        {
            get { return Application.ProductNameAndVersion; }
        }

        /// <summary>
        /// Gets the list of possibles buffer sizes for the Freebox V5
        /// </summary>
        public List<BufferSize> BufferSizesV5 { get; private set; }

        /// <summary>
        /// Gets or sets the buffer size for the Freebox V5
        /// </summary>
        public BufferSize V5BufferSize
        {
            get { return BufferSizesV5.FirstOrDefault(bf => 1024 * bf.Buffer == Settings.Default.V5BufferSize); }
            set
            {
                if (V5BufferSize != value)
                {
                    Settings.Default.V5BufferSize = value.Buffer * 1024;
                    RaisePropertyChanged(() => V5BufferSize);
                }
            }
        }

        /// <summary>
        /// Gets the list of possibles buffer sizes for the Freebox V6
        /// </summary>
        public List<BufferSize> BufferSizesV6 { get; private set; }

        /// <summary>
        /// Gets or sets the buffer size for the Freebox V6
        /// </summary>
        public BufferSize V6BufferSize
        {
            get { return BufferSizesV6.FirstOrDefault(bf => 1024 * bf.Buffer == Settings.Default.V6BufferSize); }
            set
            {
                if (V6BufferSize != value)
                {
                    Settings.Default.V6BufferSize = value.Buffer * 1024;
                    RaisePropertyChanged(() => V6BufferSize);
                }
            }
        }

        /// <summary>
        /// Gets or sets the window width
        /// </summary>
        public double WindowWidth
        {
            get { return Settings.Default.WindowWidth; }
            set
            {
                if (WindowWidth != value)
                {
                    Settings.Default.WindowWidth = value;
                    RaisePropertyChanged(() => WindowWidth);
                }
            }
        }

        /// <summary>
        /// Gets or sets the window height
        /// </summary>
        public double WindowHeight
        {
            get { return Settings.Default.WindowHeight; }
            set
            {
                if (WindowHeight != value)
                {
                    Settings.Default.WindowHeight = value;
                    RaisePropertyChanged(() => WindowHeight);
                }
            }
        }

        /// <summary>
        /// Gets or sets the TCP port number
        /// </summary>
        public int PortNumber
        {
            get { return Settings.Default.PortNumber; }
            set
            {
                if (PortNumber != value)
                {
                    Settings.Default.PortNumber = value;
                    RaisePropertyChanged(() => PortNumber);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the IPv6 is supported or not
        /// </summary>
        public bool IPv6Enabled
        {
            get { return Settings.Default.IPv6Enabled; }
            set
            {
                if (IPv6Enabled != value)
                {
                    Settings.Default.IPv6Enabled = value;
                    RaisePropertyChanged(() => IPv6Enabled);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user can delete files or not
        /// </summary>
        public bool AllowDeletion
        {
            get { return Settings.Default.AllowDeletion; }
            set
            {
                if (AllowDeletion != value)
                {
                    Settings.Default.AllowDeletion = value;
                    RaisePropertyChanged(() => AllowDeletion);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the application must check at startup if a new version is available
        /// </summary>
        public bool CheckNewVersionAvailable
        {
            get { return Settings.Default.CheckNewVersionAvailable; }
            set
            {
                if (CheckNewVersionAvailable != value)
                {
                    Settings.Default.CheckNewVersionAvailable = value;
                    RaisePropertyChanged(() => CheckNewVersionAvailable);
                }
            }
        }

        /// <summary>
        /// Gets or sets a friendly name for the server
        /// </summary>
        public string FriendlyName
        {
            get { return Settings.Default.FriendlyName; }
            set
            {
                if (FriendlyName != value)
                {
                    Settings.Default.FriendlyName = value;
                    RaisePropertyChanged(() => FriendlyName);
                }
            }
        }

        private bool _runAtWindowsStartupVisible = (Application.RunningMode != RunningMode.Service);
        /// <summary>
        /// Gets or sets a value indicating whether the RunAtWindowsStartup checkbox must be visible or not
        /// </summary>
        public bool RunAtWindowsStartupVisible
        {
            get { return _runAtWindowsStartupVisible; }
            set
            {
                if (_runAtWindowsStartupVisible != value)
                {
                    _runAtWindowsStartupVisible = value;
                    RaisePropertyChanged(() => RunAtWindowsStartupVisible);
                }
            }
        }

        private bool _runAtWindowsStartup;
        /// <summary>
        /// Gets or sets a value indicating if the application must be ran at Windows startup
        /// </summary>
        public bool RunAtWindowsStartup
        {
            get { return _runAtWindowsStartup; }
            set
            {
                if (_runAtWindowsStartup != value)
                {
                    _runAtWindowsStartup = value;
                    RaisePropertyChanged(() => RunAtWindowsStartup);
                }
            }
        }

        private TreeViewItemViewModelCollection _entries;
        /// <summary>
        /// Gets the entries
        /// </summary>
        public TreeViewItemViewModelCollection Entries
        {
            get { return _entries; }
            private set
            {
                if (_entries != value)
                {
                    _entries = value;
                    RaisePropertyChanged(() => Entries);
                }
            }
        }

        private ITreeViewItemViewModel _selectedItem;
        /// <summary>
        /// Gets or sets the selected item
        /// </summary>
        public ITreeViewItemViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    RaisePropertyChanged(() => SelectedItem);
                    OnPropertyChanged("SelectedTemplate");
                }
            }
        }

        #endregion

        #region Methods

        private void Init()
        {
            var entries = new TreeViewItemViewModelCollection(null);
            var factory = new TreeViewItemViewModelFactory();
            foreach (var entry in Settings.Default.Entries)
            {
                entries.Add(factory.CreateTreeViewItemViewModel(this, entry));
            }
            var treeViewItemViewModel = entries.FirstOrDefault();
            if (treeViewItemViewModel != null)
            {
                treeViewItemViewModel.IsSelected = true;
            }
            Entries = entries;

            if (RunAtWindowsStartupVisible && !Application.IsMono)
            {
                try
                {
                    var rkApp = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", false);
                    try
                    {
                        object value = rkApp.GetValue(Application.ProductName, false);
                        RunAtWindowsStartup = (value is string && (string)value == Application.ExecutablePath);
                    }
                    finally
                    {
                        rkApp.Close();
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// Reload the settings
        /// </summary>
        public void Reload()
        {
            try
            {
                Settings.Default.Reload();
                foreach (var property in GetType().GetProperties().Where(pi =>
                {
                    var getMethod = pi.GetGetMethod();
                    var setMethod = pi.GetSetMethod();
                    return getMethod != null && setMethod != null && getMethod.IsPublic && setMethod.IsPublic;
                }))
                {
                    OnPropertyChanged(property.Name);
                }
                Init();
            }
            catch (Exception ex)
            {
                Utils.WriteException(ex);
            }
        }

        private List<BufferSize> GetBufferSizes(int defaultBufferSize)
        {
            var bufferSizes = new List<BufferSize>();
            BufferSize bufferSize;
            for (int i = 2; i < 10; i++)
            {
                bufferSize = new BufferSize() { Buffer = (int)Math.Pow((double)2, (double)i), };
                if (bufferSize.Buffer == defaultBufferSize)
                {
                    bufferSize.Default = true;
                }
                bufferSizes.Add(bufferSize);
            }
            return bufferSizes;
        }

        /// <summary>
        /// Save the settings
        /// </summary>
        public void Save()
        {
            try
            {
                var settings = Settings.Default;
                if (RunAtWindowsStartupVisible && !Application.IsMono)
                {
                    try
                    {
                        RegistryKey rkApp = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                        try
                        {
                            object value = rkApp.GetValue(Application.ProductName, null);
                            if (RunAtWindowsStartup &&
                                (!(value is string) || (string)value != Application.ExecutablePath))
                            {
                                rkApp.SetValue(Application.ProductName, Application.ExecutablePath);
                            }
                            else
                            {
                                if (!RunAtWindowsStartup && (value is string))
                                {
                                    rkApp.DeleteValue(Application.ProductName, false);
                                }
                            }
                        }
                        finally
                        {
                            rkApp.Close();
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                settings.Entries.Clear();
                foreach (var entry in Entries)
                {
                    entry.AddModel(settings.Entries);
                }

                settings.Save();
                IsDirty = true;
            }
            catch (Exception ex)
            {
                Utils.WriteException(ex);
            }
        }

        /// <summary>
        /// Open web home page
        /// </summary>
        public void OpenHomePage()
        {
            try
            {
                Process.Start(FreeMi.UI.Properties.Resources.WebPage);
            }
            catch (Exception ex)
            {
                Utils.WriteException(ex);
            }
        }

        /// <summary>
        /// Add new folder
        /// </summary>
        /// <param name="itemKey">key of the item to add</param>
        protected void New(string itemKey)
        {
            var selectedItem = SelectedItem;
            var parentCollection = (selectedItem == null || !(selectedItem is FolderViewModel) ? Entries : ((FolderViewModel)selectedItem).Children);
            var newItem = new TreeViewItemViewModelFactory().CreateTreeViewItemViewModel(this,
                (Entry)Activator.CreateInstance(Type.GetType("FreeMi.Core.Entries." + itemKey + ", FreeMi.Core")));
            parentCollection.Add(newItem);
            if (selectedItem is FolderViewModel)
            {
                ((FolderViewModel)selectedItem).IsExpanded = true;
            }
            newItem.IsSelected = true;
        }

        /// <summary>
        /// Remove an item
        /// </summary>
        protected void Remove()
        {
            var selectedItem = SelectedItem;
            if (selectedItem != null)
            {
                selectedItem.ParentCollection.Remove(selectedItem);
            }
        }

        #endregion
    }
}