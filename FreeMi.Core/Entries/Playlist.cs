using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace FreeMi.Core.Entries
{
    /// <summary>
    /// Base class for playlists
    /// </summary>
    public abstract class Playlist : FolderBase, IFileDescription
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of a playlist
        /// </summary>
        public Playlist()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of a playlist
        /// </summary>
        /// <param name="path">path</param>
        public Playlist(string path)
            : base(path)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the file extension
        /// </summary>
        public abstract string Extension
        {
            get;
        }

        /// <summary>
        /// Gets the media kind for this file
        /// </summary>
        public MediaKind? MediaKind
        {
            get { return FreeMi.Core.MediaKind.Audio | FreeMi.Core.MediaKind.Video; }
        }

        /// <summary>
        /// Gets the label of the entry
        /// </summary>
        public override string Label
        {
            get { return String.IsNullOrWhiteSpace(Path) ? null : System.IO.Path.GetFileNameWithoutExtension(Path); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the first media url of a playlist
        /// </summary>
        /// <param name="playlist">playlist url</param>
        /// <returns>the first media url of a playlist</returns>
        public string GetMediaUrl(string playlist)
        {
            using (StreamReader reader = new StreamReader(playlist, Encoding.GetEncoding(1252)))
            {
                var browseResult = Browse(0, 1, UserAgent.Other);
                if (browseResult != null && browseResult.Entries.Count > 0)
                {
                    return browseResult.Entries[0].Path;
                }
            }
            return null;
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
            try
            {
                string path = Path;
                BrowseResult browseResult = new BrowseResult();
                using (StreamReader streamReader = new StreamReader(path, Encoding.GetEncoding(1252)))
                {
                    GetMediaFiles(streamReader, System.IO.Path.GetDirectoryName(path), browseResult.Entries, startIndex, requestedCount);
                }
                return browseResult;
            }
            catch (Exception ex)
            {
                Utils.WriteException(ex);
            }
            return null;
        }

        /// <summary>
        /// Initialization
        /// </summary>
        protected virtual void Initialize()
        {
        }

        /// <summary>
        /// Analyze file line
        /// </summary>
        /// <param name="playlistDirectory">playlist directory</param>
        /// <param name="mediaFile">file description to complete</param>
        /// <param name="line">line to analyze</param>
        /// <param name="createNewFile">function to call if the method needs to create a new file</param>
        /// <returns>true to add the file, otherwise false</returns>
        protected abstract bool AnalyzeLine(string playlistDirectory, File mediaFile, string line, Func<File> createNewFile);

        /// <summary>
        /// Gets the media files
        /// </summary>
        public void GetMediaFiles(TextReader reader, string playlistDirectory, EntryCollection entries, uint startIndex, uint requestedCount)
        {
            Initialize();

            string line;
            File mediaFile = new File();
            File newFile;
            int idx = 0;
            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();
                try
                {
                    newFile = null;
                    if (AnalyzeLine(playlistDirectory, mediaFile, line, () => newFile = new File()))
                    {
                        if (idx >= startIndex && entries.Count < requestedCount)
                        {
                            entries.Add(mediaFile);
                            if (entries.Count == requestedCount)
                            {
                                break;
                            }
                            mediaFile = newFile ?? new File();
                        }
                        idx += 1;
                    }
                }
                catch (Exception)
                {
                }
            }

            if (idx >= startIndex && entries.Count > 0 && entries.Count < requestedCount && entries[entries.Count - 1] != mediaFile &&
                !String.IsNullOrWhiteSpace(mediaFile.Path) && !String.IsNullOrWhiteSpace(mediaFile.Label))
            {
                entries.Add(mediaFile);
            }
        }

        #endregion
    }
}
