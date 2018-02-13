using FreeMi.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;

namespace FreeMi.Windows
{
    /// <summary>
    /// Trace listener where the messages will be shown in systray
    /// </summary>
    class SystrayTraceListener : TraceListener
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of SystrayTraceListener class
        /// </summary>
        /// <param name="systrayIcon">SystrayIcon</param>
        public SystrayTraceListener(SystrayIcon systrayIcon)
        {
            SystrayIcon = systrayIcon;
        }

        #endregion

        #region Properties

        private SystrayIcon SystrayIcon { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Writes the specified message to the listener
        /// </summary>
        /// <param name="message">message to show</param>
        public override void Write(string message)
        {
            SystrayIcon.ShowBalloonTip(message);
        }

        /// <summary>
        /// Writes a message to the listener followed by a line terminator.
        /// </summary>
        /// <param name="message">message to show</param>
        public override void WriteLine(string message)
        {
            Write(message);
        }

        #endregion
    }
}
