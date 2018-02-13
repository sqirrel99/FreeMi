using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;

namespace FreeMi.Core.Entries
{
    /// <summary>
    /// Base class for folders
    /// </summary>
    public abstract class FolderBase : Entry
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of a folder
        /// </summary>
        public FolderBase()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of a folder
        /// </summary>
        /// <param name="path">path</param>
        public FolderBase(string path)
            : this()
        {
            Path = path;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets an identifier
        /// </summary>
        public override string Id
        {
            get
            {
                if (UsePathAsId)
                {
                    return String.Format("{0}|{1}", GetType().Name, base.Id);
                }
                return base.Id;
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
        public abstract BrowseResult Browse(uint startIndex, uint requestedCount, UserAgent userAgent);

        /// <summary>
        /// Gets a string representing this entry
        /// </summary>
        /// <param name="receiver">receiver</param>
        /// <param name="parentId">parent identifier</param>
        /// <param name="userAgent">user agent</param>
        /// <returns>a string representing this entry</returns>
        public override string ToString(IPEndPoint receiver, string parentId, UserAgent userAgent)
        {
            var date = CreationDate;
            return String.Format(@"<container id=""{0}"" searchable=""0"" restricted=""0"" parentID=""{1}"">" +
               @"<dc:title>{2}</dc:title>" +
               (date == null ? String.Empty : @"<dc:date>{3}T{4}</dc:date>") +
               @"<upnp:class>object.container.storageFolder</upnp:class>" +
               @"<upnp:writeStatus>NOT_WRITABLE</upnp:writeStatus>" +
               @"</container>", Id, parentId, SecurityElement.Escape(Label),
               date == null ? null : ((DateTime)date).ToString("yyyy-MM-dd"),
               date == null ? null : ((DateTime)date).ToString("HH:mm:ss"));
        }

        #endregion
    }
}
