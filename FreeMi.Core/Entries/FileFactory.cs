using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FreeMi.Core.Entries
{
    static class FileFactory
    {
        private static IDictionary<string, Type> FileClasses { get; set; }

        static FileFactory()
        {
            FileClasses = new Dictionary<string, Type>();
            var fileDescriptionInterfaceName = typeof(IFileDescription).Name;
            IFileDescription fileDescription;
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes().Where(t => !t.IsAbstract && t.GetInterface(fileDescriptionInterfaceName) != null))
            {
                fileDescription = (IFileDescription)Activator.CreateInstance(type);
                FileClasses.Add(fileDescription.Extension.ToUpper(), type);
            }
        }

        public static IMedia CreateFile(string path)
        {
            try
            {
                var extension = Path.GetExtension(path);
                if (!String.IsNullOrWhiteSpace(extension))
                {
                    extension = extension.Substring(1).ToUpper();
                    Type type;
                    if (FileClasses.TryGetValue(extension, out type))
                    {
                        IMedia media = (IMedia)Activator.CreateInstance(type);
                        media.UsePathAsId = true;
                        media.Path = path;
                        return media;
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.WriteException(ex);
            }
            return new File() { UsePathAsId = true, Path = path }; ;
        }
    }
}
