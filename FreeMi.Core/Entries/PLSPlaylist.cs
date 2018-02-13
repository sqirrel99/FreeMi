using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeMi.Core.Entries
{
    /// <summary>
    /// PLS playlist
    /// </summary>
    public class PLSPlaylist : Playlist
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of PLSPlaylist class
        /// </summary>
        public PLSPlaylist()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of PLSPlaylist class
        /// </summary>
        /// <param name="path">path</param>
        public PLSPlaylist(string path)
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
            get { return "pls"; }
        }

        private int Number
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// Initialization
        /// </summary>
        protected override void Initialize()
        {
            Number = -1;
        }

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
            int number = -1;
            string str = line.ToUpperInvariant();
            string path = null, label = null, seconds = null;
            if (str.StartsWith("FILE"))
            {
                path = GetString(4, line, ref number);
            }
            else
            {
                if (str.StartsWith("TITLE"))
                {
                    label = GetString(5, line, ref number);
                }
                else
                {
                    if (str.StartsWith("LENGTH"))
                    {
                        int sec;
                        if (Int32.TryParse(GetString(6, line, ref number), out sec))
                        {
                            if (sec > 0)
                            {
                                seconds = String.Format(" ({0}:{1:00})", sec / 60, sec % 60);
                            }
                        }
                    }
                }
            }

            bool result = false;
            if (number != -1 && Number != number)
            {
                if (!String.IsNullOrWhiteSpace(mediaFile.MimeType))
                {
                    result = true;
                }
                if (Number != -1)
                {
                    mediaFile = createNewFile();
                }
                Number = number;
            }

            if (!String.IsNullOrWhiteSpace(path))
            {
                mediaFile.Path = path;
            }
            if (!String.IsNullOrWhiteSpace(label))
            {
                mediaFile.Label = (String.IsNullOrWhiteSpace(mediaFile.Label) ? label : label + mediaFile.Label);
            }
            if (!String.IsNullOrWhiteSpace(seconds))
            {
                mediaFile.Label = (String.IsNullOrWhiteSpace(mediaFile.Label) ? seconds : mediaFile.Label + seconds);
            }

            return result;
        }

        private string GetString(int startIndex, string line, ref int number)
        {
            string[] strArray = line.Split('=');
            number = Int32.Parse(strArray[0].Substring(startIndex));
            return strArray[1].Trim();
        }
    }
}
