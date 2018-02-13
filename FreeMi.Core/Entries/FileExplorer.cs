using FreeMi.Core.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FreeMi.Core.Entries
{
    /// <summary>
    /// File explorer
    /// </summary>
    public class FileExplorer : FolderBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of FileExplorer class
        /// </summary>
        public FileExplorer()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of Folder class
        /// </summary>
        /// <param name="path">path</param>
        public FileExplorer(string path)
            : base(path)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the label of the entry
        /// </summary>
        public override string Label
        {
            get { return String.IsNullOrWhiteSpace(base.Label) ? Resources.FileExplorer : base.Label; }
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
            BrowseResult browseResult = new BrowseResult();
            DriveType driveType;
            foreach (DriveInfo driveInfo in DriveInfo.GetDrives())
            {
                driveType = driveInfo.DriveType;
                if (driveType != DriveType.NoRootDirectory && driveType != DriveType.Ram &&
                    driveType != DriveType.Unknown && driveType != DriveType.Network && driveInfo.IsReady)
                {
                    if (browseResult.TotalCount >= startIndex)
                    {
                        browseResult.Entries.Add(new Folder()
                        {
                            UsePathAsId = true,
                            Label = (String.IsNullOrWhiteSpace(driveInfo.VolumeLabel) ?
                                driveInfo.Name : String.Format("{0} ({1})", driveInfo.VolumeLabel, driveInfo.Name)),
                            Path = driveInfo.RootDirectory.FullName
                        });
                    }
                    browseResult.TotalCount += 1;
                }
            }
            return browseResult;
        }

        #endregion
    }
}
