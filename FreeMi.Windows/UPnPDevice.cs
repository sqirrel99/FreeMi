using FreeMi.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeMi.Windows
{
    /// <summary>
    /// UPnP Device instance
    /// </summary>
    static class UPnPDevice
    {
        #region Properties

        private static Device _instance;
        /// <summary>
        /// Gets the device
        /// </summary>
        public static Device Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Device();
                }
                return _instance;
            }
        }

        #endregion
    }
}
