using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeMi.Core.Entries
{
    /// <summary>
    /// Playlist M3U
    /// </summary>
    public class M3UPlaylist : Playlist
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of M3UPlaylist class
        /// </summary>
        public M3UPlaylist() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of M3UPlaylist class
        /// </summary>
        /// <param name="path">path</param>
        public M3UPlaylist(string path)
            : base(path)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the file extension
        /// </summary>
        public override string Extension
        {
            get { return "m3u"; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Analyze file line
        /// </summary>
        /// <param name="playlistDirectory">playlist directory</param>
        /// <param name="mediaFile">file description to complete</param>
        /// <param name="line">line to analyze</param>
        /// <param name="createNewFile">function to call if the method needs to create a new file</param>
        /// <returns>true to add the file, otherwise false</returns>
        protected override bool AnalyzeLine(string playlistDirectory, File mediaFile, string line, Func<File> createNewFile)
        {
            if (line.StartsWith("#"))
            {
                if (line.ToUpperInvariant().StartsWith("#EXTINF:") && line.Length > 8)
                {
                    // Gets the media description
                    var description = line.Substring(8).Split(',');
                    if (description.Length == 2)
                    {
                        mediaFile.Label = description[1];
                        int seconds;
                        if (Int32.TryParse(description[0], out seconds) && seconds > 0)
                        {
                            mediaFile.Label += String.Format(" ({0}:{1:00})", seconds / 60,
                                seconds % 60);
                        }
                    }
                }
            }
            else
            {
                var mimeType = MimeType.GetMimeTypeFromFileName(line);
                if (!String.IsNullOrWhiteSpace(mimeType))
                {
                    if (MimeType.IsLiveStream(line))
                    {
                        mediaFile.Path = line;
                        if (String.IsNullOrWhiteSpace(mediaFile.Label))
                        {
                            mediaFile.Label = line;
                        }
                    }
                    else
                    {
                        Uri result;
                        mediaFile.Path = (Uri.TryCreate(line, UriKind.Absolute, out result) || String.IsNullOrWhiteSpace(playlistDirectory) ? line :
                            System.IO.Path.GetFullPath(System.IO.Path.Combine(playlistDirectory, System.IO.Path.IsPathRooted(line) ? line.Substring(System.IO.Path.GetPathRoot(line).Length) : line)));
                    }

                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}
