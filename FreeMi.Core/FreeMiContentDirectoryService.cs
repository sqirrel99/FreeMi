using FreeMi.Core.Entries;
using FreeMi.Core.Properties;
using OpenSource.UPnP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace FreeMi.Core
{
    /// <summary>
    /// ContentDirectoryService
    /// </summary>
    class FreeMiContentDirectoryService : ContentDirectoryService
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of FreeMiContentDirectoryService class
        /// </summary>
        /// <param name="id">identifier</param>
        public FreeMiContentDirectoryService(string id)
            : base(id)
        {
        }

        #endregion

        #region Properties

        private Dictionary<IPAddress, List<string>> _files = new Dictionary<IPAddress, List<string>>();
        private Dictionary<IPAddress, List<string>> Files
        {
            get { return _files; }
        }

        private int PreventSystemStandByCounter { get; set; }

        #endregion

        #region Methods

        private UserAgent GetUserAgent()
        {
            HTTPMessage headers = GetUPnPService().GetWebSession().Headers;
            if (headers != null)
            {
                string userAgent = headers.GetTag("User-Agent");
                if (!String.IsNullOrWhiteSpace(userAgent))
                {
                    if (userAgent.Contains("fbxupnpav/6.0"))
                    {
                        return UserAgent.FreeboxV6;
                    }

                    if (userAgent.Contains("fbxupnpav/1.0") || userAgent.Contains("libfbxhttp"))
                    {
                        return UserAgent.FreeboxV5;
                    }
                }
            }
            return UserAgent.Other;
        }

        /// <summary>
        /// Action: DestroyObject
        /// </summary>
        /// <param name="objectId">Associated State Variable: A_ARG_TYPE_ObjectID</param>
        public override void DestroyObject(String objectId)
        {
            if (Settings.Default.AllowDeletion)
            {
                try
                {
                    objectId = Uri.UnescapeDataString(objectId);
                    var folder = GetFolder(objectId);
                    if (folder is Folder)
                    {
                        if (folder.UsePathAsId)
                        {
                            // Folder
                            Directory.Delete(folder.Path, true);
                        }
                    }
                    else
                    {
                        // File
                        Uri uri;
                        if (Uri.TryCreate(objectId, UriKind.Absolute, out uri) && uri.IsFile)
                        {
                            System.IO.File.Delete(objectId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Utils.WriteException(ex);
                }
            }
        }

        /// <summary>
        /// Action: Browse
        /// </summary>
        /// <param name="objectId">Associated State Variable: A_ARG_TYPE_ObjectID</param>
        /// <param name="browseFlag">Associated State Variable: A_ARG_TYPE_BrowseFlag</param>
        /// <param name="filter">Associated State Variable: A_ARG_TYPE_Filter</param>
        /// <param name="startingIndex">Associated State Variable: A_ARG_TYPE_Index</param>
        /// <param name="requestedCount">Associated State Variable: A_ARG_TYPE_Count</param>
        /// <param name="sortCriteria">Associated State Variable: A_ARG_TYPE_SortCriteria</param>
        /// <param name="result">Associated State Variable: A_ARG_TYPE_Result</param>
        /// <param name="numberReturned">Associated State Variable: A_ARG_TYPE_Count</param>
        /// <param name="totalMatches">Associated State Variable: A_ARG_TYPE_Count</param>
        /// <param name="updateID">Associated State Variable: A_ARG_TYPE_UpdateID</param>
        public override void Browse(String objectId, Enum_A_ARG_TYPE_BrowseFlag browseFlag, String filter, UInt32 startingIndex,
            UInt32 requestedCount, String sortCriteria, out String result, out UInt32 numberReturned, out UInt32 totalMatches,
            out UInt32 updateID)
        {
            updateID = 0;
            numberReturned = 0;
            totalMatches = 0;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"<DIDL-Lite xmlns:dc=""http://purl.org/dc/elements/1.1/"" " +
                                      @"xmlns:upnp=""urn:schemas-upnp-org:metadata-1-0/upnp/"" " +
                                      @"xmlns=""urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/"">");
            try
            {
                FolderBase folder = (objectId == "0" || objectId == "-1") ? new MainEntry() : GetFolder(objectId);
                var userAgent = GetUserAgent();

                switch (browseFlag)
                {
                    case Enum_A_ARG_TYPE_BrowseFlag.BROWSEDIRECTCHILDREN:
                        bool isFreeboxV5 = (userAgent == UserAgent.FreeboxV5);
                        if (isFreeboxV5)
                        {
                            // In order to enchain the audio files on the Freebox V5
                            IPAddress address = GetCaller().Address;
                            if (Files.ContainsKey(address))
                            {
                                Files.Remove(address);
                            }
                        }

                        FolderBase subFolder;
                        BrowseResult browseResult;
                        bool exit = false;
                        do
                        {
                            // Browse the folder
                            browseResult = folder.Browse(startingIndex, requestedCount, userAgent);

                            if (browseResult != null && browseResult.TotalCount == 1 && browseResult.Entries.Count > 0)
                            {
                                // If there is only 1 folder, we will browse the subfolder
                                subFolder = browseResult.Entries[0] as FolderBase;
                                if (subFolder == null)
                                {
                                    exit = true;
                                }
                                else
                                {
                                    folder = subFolder;
                                }
                            }
                            else
                            {
                                exit = true;
                            }
                        }
                        while (!exit);

                        if (browseResult != null)
                        {
                            bool v6Workaround = (userAgent == UserAgent.FreeboxV6 && !folder.UsePathAsId && String.IsNullOrEmpty(folder.Path));
                            string line;
                            string dcTitle = "<dc:title>";
                            var format = dcTitle + Podcast.GetLinesNumberingFormat(browseResult.TotalCount);
                            int i = 1;
                            foreach (var entry in browseResult.Entries)
                            {
                                if (entry is Shutdown && Application.IsMono)
                                {
                                    // Unable to shutdown on Linux, Mac
                                    browseResult.TotalCount -= 1;
                                    continue;
                                }

                                line = entry.ToString(GetReceiver(), objectId, userAgent);
                                if (line == null)
                                {
                                    browseResult.TotalCount -= 1;
                                    continue;
                                }

                                if (v6Workaround)
                                {
                                    // To keep the order of the folders
                                    line = line.Replace(dcTitle, String.Format(format, i + startingIndex, String.Empty));
                                    i += 1;
                                }
                                sb.AppendLine(line);
                                numberReturned += 1;
                                AddToFileList(entry, isFreeboxV5);
                            }
                            totalMatches = (uint)browseResult.TotalCount;
                        }
                        break;

                    default:
                        sb.AppendLine((folder ?? (Entry)new Entries.File() { Path = HttpUtility.UrlDecode(objectId) }).ToString(GetReceiver(), "-1", userAgent));
                        numberReturned = 1;
                        totalMatches = 1;
                        break;
                }
            }
            catch (Exception ex)
            {
                Utils.WriteException(ex);
            }
            sb.Append(@"</DIDL-Lite>");
            result = sb.ToString();
        }

        private FolderBase GetFolder(string objectId)
        {
            try
            {
                var objectIds = objectId.Split('|');
                if (objectIds.Length > 1)
                {
                    Type folderBaseType = typeof(FolderBase);
                    string theNamespace = folderBaseType.FullName;
                    theNamespace = theNamespace.Substring(0, theNamespace.Length - folderBaseType.Name.Length);
                    List<string> args = new List<string>();
                    for (int i = 1; i < objectIds.Length; i++)
                    {
                        args.Add(HttpUtility.UrlDecode(objectIds[i]));
                    }
                    if (args.Count == 1 && String.IsNullOrEmpty(args[0]))
                    {
                        args = null;
                    }
                    var folderBase = (FolderBase)Activator.CreateInstance(Type.GetType(String.Format("{0}{1}", theNamespace, objectIds[0])), args == null ? null : args.ToArray(), null);
                    folderBase.UsePathAsId = true;
                    return folderBase;
                }
                else
                {
                    return GetFolderById(Settings.Default.Entries, objectId);
                }
            }
            catch (Exception ex)
            {
                Utils.WriteException(ex);
            }
            return null;
        }

        private FolderBase GetFolderById(EntryCollection entries, string objectId)
        {
            FolderBase folder;
            foreach (var entry in entries)
            {
                folder = entry as FolderBase;
                if (folder != null)
                {
                    if (folder.Id == objectId)
                    {
                        return folder;
                    }
                    if (folder is Folder)
                    {
                        folder = GetFolderById(((Folder)folder).Children, objectId);
                        if (folder != null)
                        {
                            return folder;
                        }
                    }
                }
            }
            return null;
        }

        private void AddToFileList(Entry entry, bool isFreeboxV5)
        {
            if (!isFreeboxV5)
            {
                return;
            }
            var file = entry as FreeMi.Core.Entries.File;
            if (file == null || file.MimeType != "audio/mpeg")
            {
                return;
            }
            IPAddress address = GetCaller().Address;
            List<string> files;
            if (Files.ContainsKey(address))
            {
                files = Files[address];
            }
            else
            {
                files = new List<string>();
                Files.Add(address, files);
            }
            files.Add(file.Path);
        }

        private void PreventSystemStandBy()
        {
            PreventSystemStandByCounter += 1;
            if (PreventSystemStandByCounter == 1)
            {
                PowerManagement.PreventSystemStandBy();
            }
        }

        private void AllowSystemStandBy()
        {
            PreventSystemStandByCounter -= 1;
            if (PreventSystemStandByCounter == 0)
            {
                PowerManagement.AllowSystemStandBy();
            }
        }

        private string GetString(string fileName, ref int idx)
        {
            int idx2 = idx;
            idx = fileName.LastIndexOf('/', idx2 - 1, idx2);
            return fileName.Substring(idx + 1, idx2 - idx - 1);
        }

        public void PacketCallBack(UPnPDevice sender, HTTPMessage msg, HTTPSession webSession, string virtualDir)
        {
            SendStreamObject(webSession, msg, msg.DirectiveObj.Substring(1));
        }

        private void SendStreamObject(HTTPSession webSession, HTTPMessage msg, string fileName)
        {
            Stream stream = null;
            try
            {
                if (!fileName.Contains("/"))
                {
                    fileName = HttpUtility.UrlDecode(fileName);
                }
                string str = (fileName[0] == '1' ? fileName.Substring(2, fileName.LastIndexOf('/') - 2) : fileName.Substring(2));
                int idx = str.Length;
                string freeboxV5 = GetString(str, ref idx);
                string file = str.Substring(0, idx);

                string mimeType = null;
                long contentLength = -1;
                bool liveStream = MimeType.IsLiveStream(file);
                string rangeStr = (msg == null ? null : msg.GetTag("RANGE"));
                List<HTTPSession.Range> rangeSets = new List<HTTPSession.Range>();
                if (!String.IsNullOrEmpty(rangeStr))
                {
                    rangeStr = rangeStr.Trim().ToLower();
                }

                if (liveStream)
                {
                    Web.CreateWebRequest(rangeSets, rangeStr, file, ref stream, ref contentLength, ref mimeType);
                    if (!String.IsNullOrEmpty(mimeType) && mimeType.ToUpperInvariant() == "AUDIO/X-SCPLS")
                    {
                        file = new PLSPlaylist().GetMediaUrl(Web.GetStringAndCloseStream(stream, Encoding.UTF8));
                        Web.CreateWebRequest(rangeSets, rangeStr, file, ref stream, ref contentLength, ref mimeType);
                    }
                }
                else
                {
                    stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                    mimeType = MimeType.GetMimeTypeFromFileName(file);
                }

                if (stream == null)
                {
                    SendErrorMessage(webSession, null);
                    return;
                }

                bool canSeek = stream.CanSeek;
                if (canSeek)
                {
                    contentLength = stream.Length;
                }

                PreventSystemStandBy();
                if (msg != null && String.Compare(msg.Directive, "HEAD", true) == 0)
                {
                    HTTPMessage head = new HTTPMessage();
                    head.StatusCode = 200;
                    head.StatusData = "OK";
                    head.ContentType = mimeType;
                    if (contentLength >= 0)
                    {
                        head.OverrideContentLength = true;
                        if (String.IsNullOrEmpty(rangeStr))
                        {
                            head.AddTag("Content-Length", contentLength.ToString());
                            head.AddTag("Accept-Ranges", "bytes");
                        }
                        else
                        {
                            head.StatusCode = 206;
                            rangeSets.AddRange(rangeStr.Trim().ToLower(), contentLength);
                            if (rangeSets.Count == 1)
                            {
                                HTTPSession.Range range = rangeSets[0];
                                head.AddTag("Content-Range", "bytes " + range.Position.ToString() + "-" + ((long)(range.Position + range.Length - 1)).ToString() + "/" + contentLength.ToString());
                                head.AddTag("Content-Length", range.Length.ToString());
                            }
                        }
                    }
                    else
                    {
                        // Can't calculate length => can't do range
                        head.AddTag("Accept-Ranges", "none");
                    }

                    webSession.Send(head);
                }
                else
                {
                    if (canSeek && contentLength >= 0 && rangeSets.Count == 0)
                    {
                        rangeSets.AddRange(rangeStr, contentLength);
                    }

                    lock (webSession)
                    {
                        webSession.OnStreamDone += new HTTPSession.StreamDoneHandler(WebSession_OnStreamDone);
                        if (freeboxV5 == "1" && mimeType == "audio/mpeg")
                        {
                            webSession.StateObject = file;
                        }

                        bool image = (MimeType.GetMediaKind(mimeType) == MediaKind.Image);
                        if (freeboxV5 == "1")
                        {
                            if (!image)
                            {
                                webSession.BUFFER_SIZE = Settings.Default.V5BufferSize;
                            }
                        }
                        else
                        {
                            webSession.BUFFER_SIZE = Settings.Default.V6BufferSize;
                        }

                        if (rangeSets.Count > 0)
                        {
                            webSession.SendStreamObject(stream, rangeSets.ToArray(), mimeType);
                        }
                        else
                        {
                            if (canSeek)
                            {
                                stream.Seek(0, SeekOrigin.Begin);
                            }
                            if (contentLength >= 0)
                            {
                                webSession.SendStreamObject(stream, contentLength, mimeType, image);
                            }
                            else
                            {
                                webSession.SendStreamObject(stream, mimeType);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.WriteException(ex);
                SendErrorMessage(webSession, stream);
            }
        }

        private void WebSession_OnStreamDone(HTTPSession sender, Stream streamObject)
        {
            sender.OnStreamDone -= WebSession_OnStreamDone;
            AllowSystemStandBy();

            try
            {
                if (streamObject != null)
                {
                    sender.CloseStreamObject(streamObject);
                    streamObject.Dispose();
                }

                string lastPlayedFile = sender.StateObject as string;
                if (!String.IsNullOrEmpty(lastPlayedFile))
                {
                    string nextFile = GetNextFile(sender.Remote.Address, lastPlayedFile);
                    if (!String.IsNullOrEmpty(nextFile))
                    {
                        // Enchain the audio files on the Freebox V5
                        SendStreamObject(sender, null, String.Format("0/{0}/1", nextFile));
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.WriteException(ex);
            }
        }

        private string GetNextFile(IPAddress address, string lastPlayedFile)
        {
            List<string> files;
            if (Files.TryGetValue(address, out files))
            {
                if (files.Count > 0)
                {
                    int idx = files.IndexOf(lastPlayedFile);
                    if (MimeType.IsLiveStream(lastPlayedFile))
                    {
                        if (idx - 1 < 0)
                        {
                            return files[files.Count - 1];
                        }
                        return files[idx - 1];
                    }

                    if (idx >= 0)
                    {
                        return files[(idx + 1) % files.Count];
                    }
                }
            }
            return null;
        }

        private void SendErrorMessage(HTTPSession webSession, Stream stream)
        {
            try
            {
                if (stream != null)
                {
                    webSession.CloseStreamObject(stream);
                    stream.Dispose();
                    stream = null;
                }

                HTTPMessage error = new HTTPMessage();
                error.StatusCode = 404;
                error.StatusData = "Error";
                error.StringBuffer = "Error";
                webSession.Send(error);
            }
            catch (Exception ex)
            {
                Utils.WriteException(ex);
            }
            finally
            {
                AllowSystemStandBy();
            }
        }

        #endregion
    }
}
