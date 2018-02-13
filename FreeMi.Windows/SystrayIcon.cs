using FreeMi.UI.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FreeMi.Windows
{
    /// <summary>
    /// To show a systray icon
    /// </summary>
    class SystrayIcon : ISystrayIcon
    {
        #region Events

        /// <summary>
        /// Occurs when the 'Restart Server' button is clicked
        /// </summary>
        public event EventHandler RestartServerClick;
        /// <summary>
        /// Occurs when the 'About' button is clicked
        /// </summary>
        public event EventHandler AboutClick;
        /// <summary>
        /// Occurs when the 'Configuration' button is clicked
        /// </summary>
        public event EventHandler ConfigurationClick;
        /// <summary>
        /// Occurs when the 'Close' button is clicked
        /// </summary>
        public event EventHandler CloseClick;

        /// <summary>
        /// Occurs when the balloon tip is clicked.
        /// </summary>
        public event EventHandler BalloonTipClicked
        {
            add { NotifyIcon.BalloonTipClicked += value; }
            remove { NotifyIcon.BalloonTipClicked -= value; }
        }

        /// <summary>
        /// Occurs when the balloon tip is closed by the user.
        /// </summary>
        public event EventHandler BalloonTipClosed
        {
            add { NotifyIcon.BalloonTipClosed += value; }
            remove { NotifyIcon.BalloonTipClosed -= value; }
        }

        /// <summary>
        /// Occurs when the user double-clicks the icon in the notification area of the taskbar.
        /// </summary>
        public event EventHandler DoubleClick
        {
            add { NotifyIcon.DoubleClick += value; }
            remove { NotifyIcon.DoubleClick -= value; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of SystrayIcon
        /// </summary>
        public SystrayIcon()
        {
            NotifyIcon = CreateNotifyIcon();
        }

        #endregion

        #region Properties

        private NotifyIcon NotifyIcon
        {
            get;
            set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Dispose the notify icon
        /// </summary>
        public void Dispose()
        {
            if (NotifyIcon != null)
            {
                NotifyIcon.Visible = false;
                NotifyIcon.Dispose();
                NotifyIcon = null;
            }
        }

        private NotifyIcon CreateNotifyIcon()
        {
            NotifyIcon notifyIcon = new NotifyIcon();
            notifyIcon.Icon = Properties.Resources.FreeMi;
            notifyIcon.Text = FreeMi.Core.Application.ProductNameAndVersion;
            notifyIcon.Visible = true;
            notifyIcon.ContextMenuStrip = BuildContextMenu();
            return notifyIcon;
        }

        private ContextMenuStrip BuildContextMenu()
        {
            ContentAlignment contentAlignment = ContentAlignment.MiddleRight;
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            ToolStripMenuItem menuItem = new ToolStripMenuItem(Resources.Configuration, Properties.Resources.FreeMi.ToBitmap());
            menuItem.Click += (sender, e) =>
            {
                if (ConfigurationClick != null)
                {
                    ConfigurationClick(this, e);
                }
            };
            menuItem.ImageAlign = contentAlignment;
            menuItem.Font = new Font(menuItem.Font, FontStyle.Bold);
            contextMenu.Items.Add(menuItem);

            contextMenu.Items.Add(new ToolStripSeparator());

            menuItem = new ToolStripMenuItem(Resources.RestartServer, Resources.RestartHS);
            menuItem.ImageAlign = contentAlignment;
            menuItem.Click += (sender, e) =>
            {
                if (RestartServerClick != null)
                {
                    RestartServerClick(this, e);
                }
            };
            contextMenu.Items.Add(menuItem);

            contextMenu.Items.Add(new ToolStripSeparator());

            menuItem = new ToolStripMenuItem(String.Format(Resources.About, Application.ProductName));
            menuItem.Click += (sender, e) =>
            {
                if (AboutClick != null)
                {
                    AboutClick(this, e);
                }
            };
            contextMenu.Items.Add(menuItem);

            contextMenu.Items.Add(new ToolStripSeparator());

            menuItem = new ToolStripMenuItem(Resources.Close);
            menuItem.Click += (sender, e) =>
            {
                if (CloseClick != null)
                {
                    CloseClick(this, e);
                }
            };
            contextMenu.Items.Add(menuItem);
            return contextMenu;
        }

        /// <summary>
        /// Displays a balloon tip in the taskbar for the specified time period
        /// </summary>
        /// <param name="text">text to show</param>
        /// <param name="toolTipIcon">toolTip icon</param>
        /// <param name="timeout">The time period, in milliseconds, the balloon tip should display</param>
        public void ShowBalloonTip(string text, ToolTipIcon toolTipIcon = ToolTipIcon.None, int timeout = 5000)
        {
            NotifyIcon.ShowBalloonTip(timeout, FreeMi.Core.Application.ProductNameAndVersion, text, toolTipIcon);
        }

        #endregion
    }
}
