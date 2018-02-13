using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeMi.Core.Entries
{
    class StrmFile : File, IFileDescription, IContainer
    {
        /// <summary>
        /// Gets the file extension
        /// </summary>
        public string Extension
        {
            get { return "strm"; }
        }

        public Entry GetContent(bool isFreeboxV5)
        {
            if (IsHidden || base.FileInfo.Length > 10240)
            {
                return null;
            }

            string path = Path;
            string strmFile = System.IO.File.ReadAllText(Path).Trim();
            string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(path);

            int startIdx = 0;
            if (strmFile.StartsWith("#V6"))
            {
                startIdx = 3;
                if (isFreeboxV5)
                {
                    // This file is not for a Freebox V5
                    return null;
                }
            }

            string mimeType = null;
            if (strmFile.IndexOf("#", startIdx) == startIdx)
            {
                startIdx += 1;
                int idx = strmFile.IndexOf('#', startIdx);
                if (idx == -1)
                {
                    strmFile = strmFile.Substring(startIdx);
                }
                else
                {
                    mimeType = strmFile.Substring(startIdx, idx - startIdx);
                    strmFile = strmFile.Substring(idx + 1, strmFile.Length - idx - 1);
                    if (mimeType == "application/rss+xml")
                    {
                        return new Podcast() { Path = strmFile, Label = fileNameWithoutExtension, UsePathAsId = true };
                    }
                    if (FreeMi.Core.MimeType.GetMediaKind(mimeType) == null)
                    {
                        mimeType = null;
                    }
                }
            }

            if (mimeType == null && System.IO.Path.GetExtension(strmFile).ToUpper() == ".XML")
            {
                return new Podcast() { Path = strmFile, Label = fileNameWithoutExtension, UsePathAsId = true };
            }

            return new File() { Path = strmFile, Label = fileNameWithoutExtension, UsePathAsId = true };
        }
    }
}
