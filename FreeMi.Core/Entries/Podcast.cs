using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace FreeMi.Core.Entries
{
    /// <summary>
    /// Podcast
    /// </summary>
    public class Podcast : FolderBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of Podcast class
        /// </summary>
        public Podcast()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of Podcast class
        /// </summary>
        /// <param name="path">path</param>
        public Podcast(string path)
            : base(path)
        {
        }

        #endregion

        #region Methods

        private string GetAttributeValue(XmlElement xmlElement, string attributName)
        {
            XmlAttribute attribute = xmlElement.Attributes[attributName];
            if (attribute != null)
            {
                return attribute.Value;
            }
            return null;
        }

        private XmlDocument GetDataFromUri(Uri uri)
        {
            var rq = WebRequest.Create(uri);
            if (rq is HttpWebRequest)
            {
                ((HttpWebRequest)rq).AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }
            rq.Timeout = 20000;
            var xmlDocument = new XmlDocument();        
            using (var responseStream = rq.GetResponse().GetResponseStream())
            {
                xmlDocument.Load(new XmlTextReader(responseStream));
            }
            return xmlDocument;
        }

        /// <summary>
        /// Browse the folder
        /// </summary>
        /// <param name="startIndex">start index</param>
        /// <param name="requestedCount">requested count</param>
        /// <param name="userAgent">user agent</param>
        /// <returns>the browse result</returns>
        public override BrowseResult Browse(uint startIndex, uint requestedCount, UserAgent userAgent)
        {
            Uri uri;
            MimeType.IsLiveStream(Path, out uri);
            var xml = GetDataFromUri(uri);
            string nodeName, url = null, mimeType = null, length = null, title;
            XmlElement urlXmlElement, titleXmlElement, dateXmlElement;
            int index;
            DateTime? date;
            DateTime parsedDate;
            bool isFreeboxV5 = (userAgent == UserAgent.FreeboxV5);
            bool isFreeboxV6 = (userAgent == UserAgent.FreeboxV6);
            var browseResult = new BrowseResult();
            if (requestedCount > 200)
            {
                requestedCount = 200;
            }
            int filesCount = 0;

            foreach (XmlNode node in xml.GetElementsByTagName("channel"))
            {
                foreach (XmlNode subNode in node)
                {
                    nodeName = subNode.Name.ToLowerInvariant();
                    if (nodeName == "item")
                    {
                        titleXmlElement = subNode["title"];
                        if (titleXmlElement == null || String.IsNullOrWhiteSpace(titleXmlElement.InnerText))
                        {
                            continue;
                        }

                        title = titleXmlElement.InnerText.Trim();
                        dateXmlElement = subNode["pubDate"];
                        date = ((dateXmlElement != null && DateTime.TryParse(dateXmlElement.InnerText, out parsedDate)) ? parsedDate : (DateTime?)null);

                        urlXmlElement = subNode["enclosure"];
                        if (urlXmlElement != null)
                        {
                            url = GetAttributeValue(urlXmlElement, "url");
                            length = GetAttributeValue(urlXmlElement, "length");
                            mimeType = GetAttributeValue(urlXmlElement, "type");
                        }
                        else
                        {
                            urlXmlElement = subNode["guid"];
                            if (urlXmlElement != null)
                            {
                                url = urlXmlElement.InnerText;
                            }
                            else
                            {
                                urlXmlElement = subNode["link"];
                                if (urlXmlElement != null)
                                {
                                    if (GetAttributeValue(urlXmlElement, "freeboxV5") != "false" || !isFreeboxV5)
                                    {
                                        if (browseResult.TotalCount >= startIndex)
                                        {
                                            browseResult.Entries.Add(new Podcast() { UsePathAsId = true, Label = title, Path = urlXmlElement.InnerText, CreationDate = date });
                                        }
                                        browseResult.TotalCount += 1;
                                        if (browseResult.Entries.Count == requestedCount)
                                        {
                                            break;
                                        }
                                        continue;
                                    }
                                }
                            }
                        }
                        if (String.IsNullOrWhiteSpace(url))
                        {
                            continue;
                        }

                        long l = 0;
                        if (!String.IsNullOrWhiteSpace(length))
                        {
                            long.TryParse(length, out l);
                        }
                        if (l == -1)
                        {
                            l = 0;
                        }

                        url = HttpUtility.UrlDecode(url);
                        index = url.IndexOf("media_url=");
                        if (index == -1)
                        {
                            index = url.LastIndexOf("http://");
                            if (index != url.IndexOf("http://"))
                            {
                                url = url.Substring(index);
                            }
                        }
                        else
                        {
                            url = url.Substring(index + 10, url.Length - index - 10);
                        }

                        string mimeTypeFromFileName = MimeType.GetMimeTypeFromFileName(url, null);
                        if (mimeTypeFromFileName != null)
                        {
                            mimeType = mimeTypeFromFileName;
                        }

                        if (browseResult.TotalCount >= startIndex)
                        {
                            browseResult.Entries.Add(new File() { Path = url, Label = title, CreationDate = date, MimeType = mimeType, Length = l });
                            filesCount += 1;
                        }
                        browseResult.TotalCount += 1;
                        if (browseResult.Entries.Count == requestedCount)
                        {
                            break;
                        }
                    }
                }

                if (browseResult.Entries.Count == requestedCount)
                {
                    break;
                }
            }

            if (isFreeboxV6 && filesCount > 0)
            {
                // Workaround on Freebox V6 to keep the order of the files
                uint i = startIndex + 1;
                File file;
                string format = GetLinesNumberingFormat(filesCount);
                foreach (var entry in browseResult.Entries)
                {
                    file = entry as File;
                    if (file != null)
                    {
                        file.Label = String.Format(format, i, file.Label);                      
                    }
                    i += 1;
                }
            }
            return browseResult;
        }

        /// <summary>
        /// Gets the string format for lines numbering
        /// </summary>
        /// <param name="count">total number of lines</param>
        /// <returns>the string format for lines numbering</returns>
        public static string GetLinesNumberingFormat(int count)
        {
            string zeros = String.Empty;
            for (var nb = 0; nb <= (int)Math.Log10(count); nb++)
            {
                zeros += "0";
            }
            return "{0:" + zeros + "} - {1}";
        }

        #endregion
    }
}
