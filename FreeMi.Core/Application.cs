using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace FreeMi.Core
{
    /// <summary>
    /// Misc informations on the application
    /// </summary>
    public static class Application
    {
        #region Native Methods

        [DllImport("libc")]
        private static extern int uname(IntPtr buf);

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating if the application is running under Mono or not
        /// </summary>
        /// <returns>true if the application is running under Mono, false otherwise</returns>
        public static bool IsMono
        {
            get { return OpenSource.UPnP.Utils.IsMono(); }
        }        

        /// <summary>
        ///  Gets a System.PlatformID enumeration value that identifies the operating system platform.
        /// </summary>
        public static PlatformID Platform
        {
            get
            {
                PlatformID platform = Environment.OSVersion.Platform;
                if (platform == PlatformID.MacOSX || platform == PlatformID.Unix)
                {
                    IntPtr buf = Marshal.AllocHGlobal(0x2000);
                    try
                    {
                        if (uname(buf) == 0 && Marshal.PtrToStringAnsi(buf) == "Darwin")
                        {
                            return PlatformID.MacOSX;
                        }
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(buf);
                    }
                    return PlatformID.Unix;
                }
                return platform;
            }
        }

        /// <summary>
        /// Gets the product name
        /// </summary>
        public static string ProductName
        {
            get
            {
                var entryAssembly = Assembly.GetEntryAssembly();
                if (entryAssembly != null)
                {
                    var customAttribute = Attribute.GetCustomAttribute(entryAssembly, typeof(AssemblyProductAttribute)) as AssemblyProductAttribute;
                    if (customAttribute != null)
                    {
                        return customAttribute.Product;
                    }
                }
                return String.Empty;
            }
        }

        /// <summary>
        /// Gets the product version
        /// </summary>
        public static Version AssemblyVersion
        {
            get { return Assembly.GetEntryAssembly().GetName().Version; }
        }

        /// <summary>
        /// Gets the product version
        /// </summary>
        public static string ProductVersion
        {
            get
            {
                var v = AssemblyVersion;
                return (v == null ? String.Empty : v.ToString(3));
            }
        }

        /// <summary>
        /// Gets the product name and version
        /// </summary>
        public static string ProductNameAndVersion
        {
            get { return String.Format("{0} {1}", ProductName, ProductVersion); }
        }

        /// <summary>
        /// Gets the assembly copyright
        /// </summary>
        public static string AssemblyCopyright
        {
            get
            {
                var entryAssembly = Assembly.GetEntryAssembly();
                if (entryAssembly != null)
                {
                    var customAttribute = Attribute.GetCustomAttribute(entryAssembly, typeof(AssemblyCopyrightAttribute)) as AssemblyCopyrightAttribute;
                    if (customAttribute != null)
                    {
                        return customAttribute.Copyright;
                    }
                }
                return String.Empty;
            }
        }

        /// <summary>
        /// Gets the executable path
        /// </summary>
        public static string ExecutablePath
        {
            get { return Path.GetFullPath(Environment.GetCommandLineArgs()[0]); }
        }

        /// <summary>
        /// Gets the startup path
        /// </summary>
        public static string StartupPath
        {
            get { return Path.GetDirectoryName(ExecutablePath); }
        }

        /// <summary>
        /// Gets the windows service name
        /// </summary>
        public static string WindowsServiceName
        {
            get { return "FreeMiWindowsService"; }
        }

        /// <summary>
        /// Gets the running mode
        /// </summary>
        public static RunningMode RunningMode
        {
            get
            {
#if PORTABLE
                return RunningMode.Portable;
#else
#if SERVICE
                return RunningMode.Service;
#else
                return RunningMode.Classic;
#endif
#endif
            }
        }

        #endregion
    }
}
