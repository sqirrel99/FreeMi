using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace FreeMi.Core
{
    /// <summary>
    /// Describes the extensions corresponding to a MIME type
    /// </summary>
    public class MimeTypeElement
    {
        #region Properties

        /// <summary>
        /// Gets or sets the MIME type
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Gets or sets the extensions
        /// </summary>
        public string Extensions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the element are subtitles
        /// </summary>
        public bool SubTitles { get; set; }

        #endregion
    }
}
