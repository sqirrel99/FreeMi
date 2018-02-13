using FreeMi.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace FreeMi.Windows
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        #region Properties

        /// <summary>
        /// Gets the systray icon manager
        /// </summary>
        private SystrayIcon SystrayIcon
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the Application object for the current application
        /// </summary>
        public static new App Current
        {
            get { return (App)System.Windows.Application.Current; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raises Startup event
        /// </summary>
        /// <param name="e">event args</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var systrayIcon = new SystrayIcon();
            SystrayIcon = systrayIcon;
            var systrayIconPresenter = new SystrayIconPresenter(systrayIcon);
            systrayIconPresenter.ShowBalloonTip(FreeMi.Core.Application.AssemblyCopyright, ToolTipIcon.None, 2000);

            Trace.Listeners.Add(new SystrayTraceListener(SystrayIcon));

            systrayIconPresenter.CheckNewVersionAvailable();
            if (Core.Application.RunningMode == RunningMode.Service)
            {
                systrayIconPresenter.ShowMainWindow();
                new WindowsServiceManager().StartService();
            }
            else
            {
                UPnPDevice.Instance.Start();
            }       
        }

        /// <summary>
        /// Raises Exit event
        /// </summary>
        /// <param name="e">event args</param>
        protected override void OnExit(ExitEventArgs e)
        {
            UPnPDevice.Instance.Stop();
            SystrayIcon.Dispose();
            SystrayIcon = null;
            base.OnExit(e);
        }

        #endregion
    }
}
