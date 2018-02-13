using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeMi.Core.Entries
{
    interface IFileDescription : IMedia
    {
        /// <summary>
        /// Gets the file extension
        /// </summary>
        string Extension { get; }
    }
}
