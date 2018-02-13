using FreeMi.Core.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;

namespace FreeMi.Core.Entries
{
    /// <summary>
    /// Computer shutdown
    /// </summary>
    public class Shutdown : FolderBase
    {
        #region Events

        /// <summary>
        /// Event raised before shutdown
        /// </summary>
        public static event EventHandler BeforeShutdown;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of Shutdown class
        /// </summary>
        public Shutdown()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of Shutdown class
        /// </summary>
        /// <param name="path">path</param>
        public Shutdown(string path)
            : base(path)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of milliseconds elapsed since the system started when the shutdown timer started
        /// </summary>
        public static int? TickCount { get; private set; }

        private static Timer _shutdownTimer;
        private static Timer ShutdownTimer
        {
            get
            {
                if (_shutdownTimer == null)
                {
                    _shutdownTimer = new Timer();
                    _shutdownTimer.Elapsed += (sender, e) =>
                    {
                        ShutdownTimer.Stop();
                        OnShutdown();
                    };
                }
                return _shutdownTimer;
            }
        }

        /// <summary>
        /// Gets or sets the label of the entry
        /// </summary>
        public override string Label
        {
            get
            {
                return String.Format("{0}{1}", String.IsNullOrWhiteSpace(base.Label) ? Resources.ShutdownComputer : base.Label,
                    TickCount == null ? String.Empty : String.Format(" (" + Resources.InXMinutes + ")", (int)(ShutdownTimer.Interval - (Environment.TickCount - TickCount)) / 60000 + 1));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Browse the folder
        /// </summary>
        /// <param name="startIndex">start index</param>
        /// <param name="requestedCount">requested count</param>
        /// <param name="userAgent">user agent</param>
        /// <returns>the browse result</returns>
        public override BrowseResult Browse(uint startIndex, uint requestedCount, UserAgent userAgent)
        {
            if (Path == null)
            {
                BrowseResult browseResult = new BrowseResult();
                AddFolder(browseResult, startIndex, requestedCount, Resources.Cancel, "ShutdownCancel");
                AddFolder(browseResult, startIndex, requestedCount, String.Format(Resources.InXMinutes, 30), "Shutdown30");
                AddFolder(browseResult, startIndex, requestedCount, String.Format(Resources.InXMinutes, 60), "Shutdown60");
                AddFolder(browseResult, startIndex, requestedCount, String.Format(Resources.InXMinutes, 90), "Shutdown90");
                AddFolder(browseResult, startIndex, requestedCount, String.Format(Resources.InXMinutes, 120), "Shutdown120");
                AddFolder(browseResult, startIndex, requestedCount, String.Format(Resources.InXMinutes, 180), "Shutdown180");
                AddFolder(browseResult, startIndex, requestedCount, String.Format(Resources.InXMinutes, 240), "Shutdown240");
                AddFolder(browseResult, startIndex, requestedCount, Resources.ShutdownImmediatly, "Shutdown");
                AddFolder(browseResult, startIndex, requestedCount, Resources.Restart, "Restart");
                return browseResult;
            }

            ShutdownTimer.Stop();
            TickCount = null;
            switch (Path)
            {
                case "ShutdownCancel":
                    break;
                case "Shutdown":
                    OnShutdown();
                    break;
                case "Restart":
                    OnShutdown(true);
                    break;
                default:
                    ShutdownTimer.Interval = Int32.Parse(Path.Substring(8)) * 60000;
                    TickCount = Environment.TickCount;
                    ShutdownTimer.Start();
                    break;
            }

            return null;
        }

        private void AddFolder(BrowseResult browseResult, uint startIndex, uint requestedCount, string label, string path)
        {
            if (browseResult.TotalCount >= startIndex && browseResult.Entries.Count < requestedCount)
            {
                browseResult.Entries.Add(new Shutdown() { UsePathAsId = true, Label = label, Path = path });
            }
            browseResult.TotalCount += 1;
        }

        /// <summary>
        /// ShutDown the computer
        /// </summary>
        private static void OnShutdown(bool restart = false)
        {
            try
            {
                if (!Application.IsMono)
                {
                    if (BeforeShutdown != null)
                    {
                        BeforeShutdown(null, EventArgs.Empty);
                    }
                    var psi = new ProcessStartInfo();
                    psi.FileName = "shutdown.exe";
                    psi.Arguments = String.Format("-{0} -f -t 0", restart ? "r" : "s");
                    psi.CreateNoWindow = true;
                    Process.Start(psi);
                }
            }
            catch (Exception)
            {
            }
        }

        #endregion
    }
}
