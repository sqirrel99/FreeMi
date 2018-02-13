using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FreeMi.Windows
{
    /// <summary>
    /// SystrayIcon interface
    /// </summary>
    interface ISystrayIcon : IDisposable
    {
        /// <summary>
        /// Occurs when the 'Restart Server' button is clicked
        /// </summary>
        event EventHandler RestartServerClick;

        /// <summary>
        /// Occurs when the 'About' button is clicked
        /// </summary>
        event EventHandler AboutClick;

        /// <summary>
        /// Occurs when the 'Configuration' button is clicked
        /// </summary>
        event EventHandler ConfigurationClick;

        /// <summary>
        /// Occurs when the 'Close' button is clicked
        /// </summary>
        event EventHandler CloseClick;

         /// <summary>
        /// Occurs when the balloon tip is clicked.
        /// </summary>
        event EventHandler BalloonTipClicked;

        /// <summary>
        /// Occurs when the balloon tip is closed by the user
        /// </summary>
        event EventHandler BalloonTipClosed;

        /// <summary>
        /// Occurs when the user double-clicks the icon in the notification area of the taskbar.
        /// </summary>
        event EventHandler DoubleClick;

        /// <summary>
        /// Displays a balloon tip in the taskbar for the specified time period
        /// </summary>
        /// <param name="text">text to show</param>
        /// <param name="toolTipIcon">toolTip icon</param>
        /// <param name="timeout">The time period, in milliseconds, the balloon tip should display</param>
        void ShowBalloonTip(string text, ToolTipIcon toolTipIcon = ToolTipIcon.None, int timeout = 5000);
    }
}
