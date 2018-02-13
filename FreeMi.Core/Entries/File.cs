using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Xml.Serialization;

namespace FreeMi.Core.Entries
{
    /// <summary>
    /// Base class for file entries
    /// </summary>
    public class File : Entry, IMedia
    {
        /// <summary>
        /// Gets a value indicating wether the folder is a hidden folder or not
        /// </summary>
        public bool IsHidden
        {
            get
            {
                var fileInfo = FileInfo;
                if (fileInfo == null)
                {
                    if (IsLiveStream)
                    {
                        return false;
                    }
                    return true;
                }
                return (fileInfo.Attributes != (FileAttributes)(-1) &&
                    (fileInfo.Attributes & FileAttributes.Hidden) != 0);
            }
        }

        /// <summary>
        /// Gets a value indicating wether the file is a live stream or not
        /// </summary>
        protected bool IsLiveStream { get; private set; }

        private FileInfo _fileInfo;
        private bool _fileInfoInitialized;
        /// <summary>
        /// Gets some informations on the file
        /// </summary>
        protected FileInfo FileInfo
        {
            get
            {
                if (!_fileInfoInitialized)
                {
                    _fileInfoInitialized = true;
                    if (!String.IsNullOrWhiteSpace(Path))
                    {
                        try
                        {
                            IsLiveStream = Core.MimeType.IsLiveStream(Path);
                            if (!IsLiveStream)
                            {
                                _fileInfo = new FileInfo(Path);
                            }
                        }
                        catch (Exception ex)
                        {
                            Utils.WriteException(ex);
                            return null;
                        }
                    }
                }
                return _fileInfo;
            }
        }

        /// <summary>
        /// Gets or sets the path of the entry
        /// </summary>
        public override string Path
        {
            get { return base.Path; }
            set
            {
                if (base.Path != value)
                {
                    base.Path = value;
                    IsLiveStream = false;
                    _fileInfoInitialized = false;
                    _mimeType = null;
                    _mediaKind = null;
                }
            }
        }

        /// <summary>
        /// Gets the label of the entry
        /// </summary>
        public override string Label
        {
            get { return String.IsNullOrWhiteSpace(base.Label) ? (FileInfo == null ? null : FileInfo.Name) : base.Label; }
        }

        private long _length;
        /// <summary>
        /// Gets the length of the file
        /// </summary>
        [XmlIgnore]
        public long Length
        {
            get { return _length == 0 ? (FileInfo == null ? 0 : FileInfo.Length) : _length; }
            set { _length = value; }
        }

        /// <summary>
        /// Gets the creation date
        /// </summary>
        public override DateTime? CreationDate
        {
            get { return base.CreationDate ?? (FileInfo == null ? (DateTime?)null : FileInfo.CreationTime); }
        }

        private MediaKind? _mediaKind;
        /// <summary>
        /// Gets the media kind of this file
        /// </summary>
        public MediaKind? MediaKind
        {
            get
            {
                if (IsHidden)
                {
                    return null;
                }
                if (_mediaKind == null)
                {
                    _mediaKind = FreeMi.Core.MimeType.GetMediaKind(MimeType);
                }
                return _mediaKind;
            }
        }

        private string _mimeType;
        /// <summary>
        /// Gets the MIME type for this file
        /// </summary>
        [XmlIgnore]
        public string MimeType
        {
            get
            {
                if (_mimeType == null)
                {
                    _mimeType = FreeMi.Core.MimeType.GetMimeTypeFromFileName(Path, null);
                }
                return _mimeType;
            }
            set { _mimeType = value; }
        }

        /// <summary>
        /// Gets a string representing this entry
        /// </summary>
        /// <param name="receiver">receiver</param>
        /// <param name="parentId">parent identifier</param>
        /// <param name="userAgent">user agent</param>
        /// <returns>a string representing this entry</returns>
        public override string ToString(IPEndPoint receiver, string parentId, UserAgent userAgent)
        {
            string mimeType = MimeType;
            if (string.IsNullOrWhiteSpace(mimeType))
            {
                return null;
            }

            bool freeboxV5 = (userAgent == UserAgent.FreeboxV5);
            if (freeboxV5 && mimeType == "audio/mpeg")
            {
                // Workaround on Freebox V5 to read MP3 files
                mimeType = "video/avi";
            }
            string objectItem = FreeMi.Core.MimeType.GetObjectItem((MediaKind)MediaKind);
            var date = CreationDate;
            bool showLabel = (freeboxV5 && MediaKind == FreeMi.Core.MediaKind.Image);

            return String.Format(@"<item id=""{0}"" restricted=""0"" parentID=""{1}"">" +
               @"<dc:title>{2}</dc:title>" +
               (date == null ? String.Empty : @"<dc:date>{9}T{10}</dc:date>") +
               @"<upnp:class>object.item.{7}</upnp:class>" +
               @"<res size=""{8}"" protocolInfo=""http-get:*:{6}:*"">http://{5}/FreeMi/{3}{4}</res>" +
               @"</item>",
               Uri.EscapeDataString(Path),
               parentId,
               SecurityElement.Escape(Label),
               Uri.EscapeDataString(String.Format("{0}/{1}/{2}", showLabel ? "1" : "0", Path, freeboxV5 ? "1" : "0")),
               showLabel ? "/" + Uri.EscapeDataString(Label.Replace('/', '_')) : String.Empty,
               receiver,
               mimeType,
               objectItem,
               Length,
               date == null ? String.Empty : ((DateTime)date).ToString("yyyy-MM-dd"),
               date == null ? String.Empty : ((DateTime)date).ToString("HH:mm:ss"))
            ;
        }
    }
}
