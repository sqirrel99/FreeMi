using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeMi.Core.Entries
{
    /// <summary>
    /// Browse result
    /// </summary>
    public class BrowseResult
    {
        private EntryCollection _entries = new EntryCollection();
        /// <summary>
        /// Gets the requested entries
        /// </summary>
        public EntryCollection Entries { get { return _entries; } }

        /// <summary>
        /// Gets or sets the total number of entries
        /// </summary>
        public int TotalCount { get; set; }
    }
}
