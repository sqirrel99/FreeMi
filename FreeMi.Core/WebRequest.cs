using OpenSource.UPnP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace FreeMi.Core
{
    /// <summary>
    /// Web helper class
    /// </summary>
    public static class Web
    {
        internal static void CreateWebRequest(List<HTTPSession.Range> rangeSets, string rangeStr, string file, ref Stream stream,
           ref long contentLength, ref string mimeType)
        {
            if (!String.IsNullOrEmpty(file) && !file.ToUpperInvariant().StartsWith("MMS://"))
            {
                try
                {
                    var webRequest = System.Net.WebRequest.Create(file);
                    var httpWebRequest = webRequest as HttpWebRequest;
                    var addRange = false;
                    long range = -1;
                    if (httpWebRequest != null)
                    {
                        httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                        if (!String.IsNullOrEmpty(rangeStr))
                        {
                            var args = new object[3];
                            var idx = rangeStr.IndexOf('=');
                            if (idx == -1)
                            {
                                idx = rangeStr.IndexOf(' ');
                            }
                            args[0] = rangeStr.Substring(0, idx).Trim();
                            var idx2 = rangeStr.IndexOf('-');
                            args[1] = rangeStr.Substring(idx + 1, idx2 - idx - 1).Trim();
                            range = long.Parse((string)args[1]);
                            idx = rangeStr.IndexOf('/');
                            if (idx == -1)
                            {
                                idx = rangeStr.Length;
                            }
                            var length = idx - idx2 - 1;
                            if (length == 0)
                            {
                                using (var r = System.Net.WebRequest.Create(file).GetResponse())
                                {
                                    args[2] = r.ContentLength <= 0 ? String.Empty : (r.ContentLength - 1).ToString();
                                }
                            }
                            else
                            {
                                args[2] = rangeStr.Substring(idx2 + 1, length).Trim();
                            }

                            if (Application.IsMono)
                            {
                                // Appel de httpWebRequest.Headers.RemoveAndAdd(string name, string value);
                                typeof(WebHeaderCollection).GetMethod("RemoveAndAdd", BindingFlags.Instance | BindingFlags.NonPublic).
                                    Invoke(httpWebRequest.Headers, new object[] { "Range", String.Format("{0}={1}-{2}", args[0], args[1], args[2]) });
                            }
                            else
                            {
                                // Appel de httpWebRequest.AddRange(string rangeSpecifier, long from, long to);
                                MethodInfo mi = null;
                                MethodInfo[] methodInfos = httpWebRequest.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
                                foreach (MethodInfo methodInfo in methodInfos)
                                {
                                    if (methodInfo.Name == "AddRange")
                                    {
                                        mi = methodInfo;
                                        break;
                                    }
                                }
                                mi.Invoke(httpWebRequest, args);
                            }

                            addRange = true;
                        }
                    }
                    var response = webRequest.GetResponse();
                    stream = response.GetResponseStream();
                    contentLength = response.ContentLength;
                    if (addRange && contentLength > 0)
                    {
                        rangeSets.Add(new HTTPSession.Range(range, contentLength));
                    }
                    if (!String.IsNullOrEmpty(response.ContentType))
                    {
                        mimeType = response.ContentType;
                    }
                }
                catch (NotSupportedException)
                {
                }
            }
        }

        /// <summary>
        /// Download a string
        /// </summary>
        /// <param name="request">Uri</param>
        /// <returns>a string</returns>
        public static string DownloadString(Uri request)
        {
            return GetStringAndCloseStream(WebRequest.Create(request).GetResponse().GetResponseStream(), Encoding.ASCII);
        }

        /// <summary>
        /// Gets a string
        /// </summary>
        /// <param name="sstream">stream</param>
        /// <param name="encoding">encoding</param>
        /// <returns>the stream</returns>
        public static string GetStringAndCloseStream(Stream sstream, Encoding encoding)
        {
            StringBuilder sb = new StringBuilder();
            using (Stream stream = sstream)
            {
                byte[] buffer = new byte[4096];
                int nb;
                while ((nb = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    sb.Append(encoding.GetString(buffer, 0, nb));
                }
            }
            return sb.ToString();
        }
    }
}
