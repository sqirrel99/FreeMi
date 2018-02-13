using FreeMi.Core;
using FreeMi.Core.Properties;
using FreeMi.UI.Properties;
using FreeMi.UI.ViewModels;
using FreeMi.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace FreeMi.Windows
{
    /// <summary>
    /// Presenter for SystrayIcon
    /// </summary>
    class SystrayIconPresenter
    {
        #region Properties

        private ISystrayIcon SystrayIcon
        {
            get;
            set;
        }

        private MainViewModel _mainViewModel;
        private MainViewModel MainViewModel
        {
            get
            {
                if (_mainViewModel == null)
                {
                    _mainViewModel = new MainViewModel();
                }
                return _mainViewModel;
            }
        }

        private bool IsShutdowning { get; set; }

        private MainWindow _mainWindow;
        private MainWindow MainWindow
        {
            get
            {
                if (_mainWindow == null)
                {
                    _mainWindow = new MainWindow();
                    _mainWindow.IsVisibleChanged += (sender, e) => { if (e.NewValue.Equals(false)) OnMainWindowClosed(); };
                    _mainWindow.Closed += (sender, e) => OnMainWindowClosed();
                    _mainWindow.DataContext = MainViewModel;
                }
                return _mainWindow;
            }
        }        

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of SystrayIconPresenter
        /// </summary>
        /// <param name="systrayIcon">ISystrayIcon interface</param>
        public SystrayIconPresenter(ISystrayIcon systrayIcon)
        {
            SystrayIcon = systrayIcon;
            systrayIcon.AboutClick += AboutClick;
            systrayIcon.RestartServerClick += RestartServerClick;
            systrayIcon.CloseClick += CloseClick;
            systrayIcon.ConfigurationClick += ConfigurationClick;
            systrayIcon.DoubleClick += ConfigurationClick;
        }

        private void OnMainWindowClosed()
        {
            MainViewModel.Save();
            if (MainViewModel.IsDirty && (FreeMi.Core.Application.RunningMode == RunningMode.Service || !IsShutdowning))
            {
                Restart();
            }
        }

        private void CloseClick(object sender, EventArgs e)
        {
            Close();
        }

        private void ConfigurationClick(object sender, EventArgs e)
        {
            ShowMainWindow();
        }

        private bool Restart()
        {
            if (FreeMi.Core.Application.RunningMode == RunningMode.Service)
            {
                return new WindowsServiceManager().RestartService();
            }
            else
            {
                return UPnPDevice.Instance.Restart();
            }
        }

        private void RestartServerClick(object sender, EventArgs e)
        {
            if (Restart())
            {
                ShowBalloonTip(UI.Properties.Resources.RestartedServer, ToolTipIcon.Info);
            }
        }

        private void AboutClick(object sender, EventArgs e)
        {
            SystrayIcon.BalloonTipClicked += new EventHandler(NotifyIconBalloonTipClicked);
            SystrayIcon.BalloonTipClosed += new EventHandler(NotifyIconBalloonTipClosed);
            ShowBalloonTip(UI.Properties.Resources.AboutBox, ToolTipIcon.Info, 30000);
        }

        private void RemoveBalloonEventHandlers()
        {
            SystrayIcon.BalloonTipClicked -= new EventHandler(NotifyIconBalloonTipClicked);
            SystrayIcon.BalloonTipClosed -= new EventHandler(NotifyIconBalloonTipClosed);
        }

        private void NotifyIconBalloonTipClicked(object sender, EventArgs e)
        {
            RemoveBalloonEventHandlers();
            MainViewModel.OpenHomePage();
        }

        private void NotifyIconBalloonTipClosed(object sender, EventArgs e)
        {
            RemoveBalloonEventHandlers();
        }      

        /// <summary>
        /// Displays a balloon tip in the taskbar for the specified time period
        /// </summary>
        /// <param name="text">text to show</param>
        /// <param name="toolTipIcon">toolTip icon</param>
        /// <param name="timeout">The time period, in milliseconds, the balloon tip should display</param>
        public void ShowBalloonTip(string text, ToolTipIcon toolTipIcon = ToolTipIcon.None, int timeout = 5000)
        {
            SystrayIcon.ShowBalloonTip(text, toolTipIcon, timeout);
        }

        /// <summary>
        /// Check if a new version of the application is available
        /// </summary>
        public void CheckNewVersionAvailable()
        {
            if (Settings.Default.CheckNewVersionAvailable)
            {
                try
                {
                    var bw = new BackgroundWorker();
                    bw.DoWork += new DoWorkEventHandler(BackgroundWorker_DoWork);
                    bw.RunWorkerAsync();
                }
                catch (Exception)
                {
                }
            }
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                string version = Web.DownloadString(new Uri(UI.Properties.Resources.VersionFileUrl));
                if (!String.IsNullOrEmpty(version))
                {
                    Version newVersion = new Version(version.Trim());
                    if (newVersion > FreeMi.Core.Application.AssemblyVersion)
                    {
                        if (System.Windows.MessageBox.Show(String.Format(UI.Properties.Resources.NewVersionAvailableMessage, 
                            FreeMi.Core.Application.ProductName, newVersion, FreeMi.Core.Application.ProductVersion),
                            UI.Properties.Resources.NewVersionAvailable, 
                            MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
                        {
                            Process.Start(UI.Properties.Resources.DownloadWebPage);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Show the main window
        /// </summary>
        public void ShowMainWindow()
        {
            var mainWindow = MainWindow;
            if (mainWindow.IsVisible)
            {
                if (mainWindow.WindowState == WindowState.Minimized)
                {
                    mainWindow.WindowState = WindowState.Normal;
                }
            }
            else
            {
                MainViewModel.Reload();
            }
            mainWindow.Show();
            mainWindow.Activate();
            mainWindow.Topmost = true;
            mainWindow.Topmost = false;
            mainWindow.Focus();
        }

        /// <summary>
        /// Close the application
        /// </summary>
        public void Close()
        {
            IsShutdowning = true;
            try
            {
                System.Windows.Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                Utils.WriteException(ex);
            }
            finally
            {
                IsShutdowning = false;
            }
        }

        #endregion     
    }
}
