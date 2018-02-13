using FreeMi.UI.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeMi.UI.ViewModels
{
    /// <summary>
    /// Buffer size
    /// </summary>
    public class BufferSize
    {
        #region Properties

        /// <summary>
        /// Gets a sets a value indicating whether this buffer size is the default value or not
        /// </summary>
        public bool Default { get; set; }

        /// <summary>
        /// Gets or sets the buffer size
        /// </summary>
        public int Buffer { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <returns>A string that represents the current object</returns>
        public override string ToString()
        {
            return Default ? String.Format("{0} ({1})", Buffer, Resources.ByDefault) :
                Buffer.ToString();
        }

        #endregion
    }
}
