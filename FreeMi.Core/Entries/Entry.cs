using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace FreeMi.Core.Entries
{
    /// <summary>
    /// Entry base class
    /// </summary>
    [XmlInclude(typeof(File))]
    [XmlInclude(typeof(Folder))]
    [XmlInclude(typeof(FileExplorer))]
    [XmlInclude(typeof(M3UPlaylist))]
    [XmlInclude(typeof(PLSPlaylist))]
    [XmlInclude(typeof(Podcast))]
    [XmlInclude(typeof(Shutdown))]
    public abstract class Entry
    {
        /// <summary>
        /// Gets or sets a value indicating whether the identifier must be the path
        /// </summary>
        [XmlIgnore]
        public bool UsePathAsId { get; set; }

        private Guid _id = Guid.Empty;
        /// <summary>
        /// Gets an identifier
        /// </summary>
        public virtual string Id
        {
            get
            {
                if (UsePathAsId)
                {
                    return Uri.EscapeDataString(Path);
                }

                if (_id == Guid.Empty)
                {
                    _id = Guid.NewGuid();
                }
                return _id.ToString();
            }
            set
            {
                if (!UsePathAsId)
                {
                    _id = Guid.Parse(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the label of the entry
        /// </summary>
        public virtual string Label { get; set; }

        /// <summary>
        /// Gets or sets the path of the entry
        /// </summary>
        public virtual string Path { get; set; }

        /// <summary>
        /// Gets the creation date
        /// </summary>
        [XmlIgnore]
        public virtual DateTime? CreationDate { get; set; }

        /// <summary>
        /// Gets a string representing this entry
        /// </summary>
        /// <param name="receiver">receiver</param>
        /// <param name="parentId">parent identifier</param>
        /// <param name="userAgent">user agent</param>
        /// <returns>a string representing this entry</returns>
        public abstract string ToString(IPEndPoint receiver, string parentId, UserAgent userAgent);
    }
}
