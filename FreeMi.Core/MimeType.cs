using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace FreeMi.Core
{
    /// <summary>
    /// MimeType manager
    /// </summary>
    public static class MimeType
    {
        #region Properties

        private static Dictionary<string, string> m_extensions;
        private static Dictionary<string, string> Extensions
        {
            get
            {
                if (m_extensions == null)
                {
                    m_extensions = new Dictionary<string, string>();
                }
                return m_extensions;
            }
        }

        private static List<string> m_subTitles;
        private static List<string> SubTitles
        {
            get
            {
                if (m_subTitles == null)
                {
                    m_subTitles = new List<string>();
                }
                return m_subTitles;
            }
        }

        #endregion

        /// <summary>
        /// Check if the media is a live stream
        /// </summary>
        /// <param name="uriString">uri</param>
        /// <returns>true if the media is a live stream, false otherwise</returns>
        public static bool IsLiveStream(string uriString)
        {
            Uri uri;
            return IsLiveStream(uriString, out uri);
        }

        /// <summary>
        /// Check if the media is a live stream
        /// </summary>
        /// <param name="uriString">uri</param>
        /// <param name="uri">result Uri</param>
        /// <returns>true if the media is a live stream, false otherwise</returns>
        public static bool IsLiveStream(string uriString, out Uri uri)
        {
            return Uri.TryCreate(uriString, UriKind.Absolute, out uri) && !uri.IsFile;
        }

        /// <summary>
        /// Gets the mime type from a filename
        /// </summary>
        /// <param name="fileName">filename</param>
        /// <returns>the corresponding mime type</returns>
        public static string GetMimeTypeFromFileName(string fileName)
        {
            return GetMimeTypeFromFileName(fileName, ".MP3");
        }

        /// <summary>
        /// Gets the media kind
        /// </summary>
        /// <param name="mimeType">MIME type</param>
        /// <returns>the media kind</returns>
        public static MediaKind? GetMediaKind(string mimeType)
        {
            if (mimeType == null)
            {
                return null;
            }
            if (mimeType.StartsWith("audio/") || mimeType.StartsWith("application/ogg"))
            {
                return MediaKind.Audio;
            }
            if (mimeType.StartsWith("video/"))
            {
                return MediaKind.Video;
            }
            if (mimeType.StartsWith("image/"))
            {
                return MediaKind.Image;
            }
            if (SubTitles.BinarySearch(mimeType) >= 0)
            {
                return MediaKind.Video | MediaKind.Other;
            }
            return MediaKind.Other;
        }

        /// <summary>
        /// Gets the object item kind
        /// </summary>
        public static string GetObjectItem(MediaKind mediaKind)
        {
            switch (mediaKind)
            {
                case MediaKind.Video:
                    return "videoItem.movie";
                case MediaKind.Image:
                    return "imageItem.photo";
                case MediaKind.Audio:
                    return "audioItem.musicTrack";
                default:
                    return "textItem";
            }
        }

        /// <summary>
        /// Gets the mime type from a filename
        /// </summary>
        /// <param name="fileName">filename</param>
        /// <param name="defaultExtension">default extension</param>
        /// <returns>the corresponding mime type</returns>
        public static string GetMimeTypeFromFileName(string fileName, string defaultExtension)
        {
            string extension;
            try
            {
                extension = Path.GetExtension(fileName);
            }
            catch (Exception ex)
            {
                Utils.WriteException(ex);
                extension = defaultExtension;
            }

            if (String.IsNullOrEmpty(extension))
            {
                if (String.IsNullOrEmpty(defaultExtension))
                {
                    return IsLiveStream(fileName) ? "audio/mp3" : null;
                }
                return GetMimeType(defaultExtension);
            }

            return GetMimeType(extension);
        }

        private static string GetMimeType(string ext)
        {
            var extensions = Extensions;
            if (extensions.Count == 0)
            {
                MimeTypeElement[] mimeTypes;
                try
                {
                    var xmlSerializer = new XmlSerializer(typeof(MimeTypeElement[]));
                    using (var xmlReader = XmlReader.Create(Path.Combine(Application.StartupPath, "MimeTypes.xml")))
                    {
                        mimeTypes = xmlSerializer.Deserialize(xmlReader) as MimeTypeElement[];
                    }
                }
                catch (Exception)
                {
                    mimeTypes = null;
                }
                if (mimeTypes == null)
                {
                    return null;
                }
                string e;
                var subTitles = SubTitles;
                foreach (MimeTypeElement element in mimeTypes)
                {
                    foreach (string extension in element.Extensions.Split(';'))
                    {
                        e = extension.Trim();
                        if (!String.IsNullOrEmpty(e))
                        {
                            extensions.Add(e, element.MimeType);
                        }
                    }

                    if (element.SubTitles)
                    {
                        subTitles.Add(element.MimeType);
                    }
                }
                subTitles.Sort();
            }

            string mimeType;
            if (extensions.TryGetValue(ext.Replace(".", "").Trim().ToLowerInvariant(), out mimeType))
            {
                return mimeType;
            }
            return null;
        }
    }
}
