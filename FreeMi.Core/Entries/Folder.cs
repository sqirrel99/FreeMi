using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FreeMi.Core.Entries
{
    /// <summary>
    /// Folder class
    /// </summary>
    public class Folder : FolderBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of Folder class
        /// </summary>
        public Folder()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of Folder class
        /// </summary>
        /// <param name="path">path</param>
        /// <param name="mediaKind">media kind</param>
        public Folder(string path, string mediaKind)
            : base(path)
        {
            MediaKind = (MediaKind)int.Parse(mediaKind);
        }

        #endregion

        #region Properties

        private EntryCollection _children;
        /// <summary>
        /// Gets the child entries of this folder
        /// </summary>
        public EntryCollection Children
        {
            get
            {
                if (_children == null)
                {
                    _children = new EntryCollection();
                }
                return _children;
            }
        }

        /// <summary>
        /// Gets an identifier
        /// </summary>
        public override string Id
        {
            get
            {
                if (UsePathAsId)
                {
                    return String.Format("{0}|{1}", base.Id, (int)MediaKind);
                }
                return base.Id;
            }
        }

        private MediaKind _mediaKind = MediaKind.All;
        /// <summary>
        /// Gets or sets the media kind for this folder
        /// </summary>
        public MediaKind MediaKind
        {
            get { return _mediaKind; }
            set { _mediaKind = value; }
        }

        /// <summary>
        /// Gets a value indicating wether the folder is a hidden folder or not
        /// </summary>
        public bool IsHidden
        {
            get
            {
                var directoryInfo = DirectoryInfo;
                if (directoryInfo == null) { return true; }
                return (directoryInfo.Attributes != (FileAttributes)(-1) &&
                    (directoryInfo.Attributes & FileAttributes.Hidden) != 0);
            }
        }

        private DirectoryInfo _directoryInfo;
        private bool _directoryInfoInitialized;
        private DirectoryInfo DirectoryInfo
        {
            get
            {
                if (!_directoryInfoInitialized)
                {
                    _directoryInfoInitialized = true;
                    if (!String.IsNullOrWhiteSpace(Path))
                    {
                        try
                        {
                            _directoryInfo = new DirectoryInfo(Path);
                        }
                        catch (Exception ex)
                        {
                            Utils.WriteException(ex);
                            return null;
                        }
                    }
                }
                return _directoryInfo;
            }
        }

        /// <summary>
        /// Gets or sets the path of the entry
        /// </summary>
        public override string Path
        {
            get { return base.Path; }
            set
            {
                if (base.Path != value)
                {
                    base.Path = value;
                    _directoryInfoInitialized = false;
                }
            }
        }

        /// <summary>
        /// Gets the label of the entry
        /// </summary>
        public override string Label
        {
            get { return String.IsNullOrWhiteSpace(base.Label) ? (DirectoryInfo == null ? null : DirectoryInfo.Name) : base.Label; }
        }

        /// <summary>
        /// Gets the creation date
        /// </summary>
        public override DateTime? CreationDate
        {
            get { return base.CreationDate ?? (DirectoryInfo == null ? (DateTime?)null : DirectoryInfo.CreationTime); }
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
            var entries = new EntryCollection();
            entries.AddRange(Children);
            if (!String.IsNullOrWhiteSpace(Path))
            {
                try
                {
                    bool isFreeboxV5 = (userAgent == UserAgent.FreeboxV5);
                    foreach (var file in Directory.GetFiles(Path, "*.*", SearchOption.TopDirectoryOnly))
                    {
                        try
                        {
                            var f = FileFactory.CreateFile(file);
                            if (f is IContainer)
                            {
                                var entry = ((IContainer)f).GetContent(isFreeboxV5);
                                if (entry == null)
                                {
                                    continue;
                                }
                                if (!(entry is IMedia))
                                {
                                    entries.Add(entry);
                                    continue;
                                }
                                f = (IMedia)entry;
                            }

                            if (f.MediaKind != null && (f.MediaKind & MediaKind) > 0)
                            {
                                entries.Add((Entry)f);
                            }
                        }
                        catch (Exception ex)
                        {
                            Utils.WriteException(ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Utils.WriteException(ex);
                }

                try
                {
                    foreach (var directory in Directory.GetDirectories(Path, "*.*", SearchOption.TopDirectoryOnly))
                    {
                        try
                        {
                            var folder = new Folder() { UsePathAsId = true, Path = directory };
                            if (!folder.IsHidden)
                            {
                                entries.Add(folder);
                            }
                        }
                        catch (Exception ex)
                        {
                            Utils.WriteException(ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Utils.WriteException(ex);
                }
            }

            entries.Sort(new Comparison<Entry>((e1, e2) =>
            {
                if (e1 is FolderBase && !(e2 is FolderBase)) { return -1; }
                if (e2 is FolderBase && !(e1 is FolderBase)) { return 1; }
                if (e1.Label == null) { return e2.Label == null ? 0 : -1; }
                return e1.Label.CompareTo(e2.Label);
            }));

            var browseResult = new BrowseResult() { TotalCount = entries.Count };
            int i = 0;
            foreach (Entry entry in entries)
            {
                if (i == startIndex)
                {
                    if (browseResult.Entries.Count < requestedCount)
                    {
                        browseResult.Entries.Add(entry);
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    i += 1;
                }
            }
            return browseResult;
        }

        #endregion
    }
}