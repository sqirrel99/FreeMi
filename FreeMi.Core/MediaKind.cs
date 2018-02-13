using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeMi.Core
{
    /// <summary>
    /// Media kind
    /// </summary>
    [Flags]
    public enum MediaKind
    {
        /// <summary>
        /// Video
        /// </summary>
        Video = 1,
        /// <summary>
        /// Audio
        /// </summary>
        Audio = 2,
        /// <summary>
        /// Image
        /// </summary>
        Image = 4,
        /// <summary>
        /// Other
        /// </summary>
        Other = 16,
        /// <summary>
        /// All
        /// </summary>
        All = Video | Audio | Image | Other
    }
}
