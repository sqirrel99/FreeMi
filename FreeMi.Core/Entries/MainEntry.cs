using FreeMi.Core.Properties;
using System.Linq;

namespace FreeMi.Core.Entries
{
    /// <summary>
    /// Main menu
    /// </summary>
    public class MainEntry : FolderBase
    {
        #region Properties

        /// <summary>
        /// Gets an identifier
        /// </summary>
        public override string Id
        {
            get { return "0"; }
        }

        /// <summary>
        /// Gets the label of the entry
        /// </summary>
        public override string Label
        {
            get { return Application.ProductName; }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Browse the folder
        /// </summary>
        /// <param name="startIndex">start index</param>
        /// <param name="requestedCount">requested count</param>
        /// <param name="userAgent">user agent</param>
        /// <returns>the browse result</returns>
        public override BrowseResult Browse(uint startIndex, uint requestedCount, UserAgent userAgent)
        {
            var browseResult = new BrowseResult();
            var defaultEntries = Settings.Default.Entries;
            browseResult.TotalCount = defaultEntries.Count();
            int i = 0;
            foreach (var entry in defaultEntries)
            {
                if (i >= startIndex)
                {
                    browseResult.Entries.Add(entry);
                    if (browseResult.Entries.Count == requestedCount)
                    {
                        break;
                    }
                }
                i += 1;
            }
            return browseResult;
        }

        #endregion
    }
}
