using FreeMi.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace FreeMi.Windows
{
    /// <summary>
    /// Windows service manager
    /// </summary>
    class WindowsServiceManager
    {
        #region Properties

        private ServiceController _serviceController;
        private ServiceController ServiceController
        {
            get
            {
                if (_serviceController == null)
                {
                    _serviceController = new ServiceController(Application.WindowsServiceName);
                }
                return _serviceController;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Windows service start
        /// </summary>
        public bool StartService()
        {
            try
            {
                ServiceController.Refresh();
                if (ServiceController.Status == ServiceControllerStatus.Stopped)
                {
                    ServiceController.Start();
                    ServiceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                }
                return true;
            }
            catch (Exception ex)
            {
                Utils.WriteException(ex);
            }
            return false;
        }

        /// <summary>
        /// Windows service stop
        /// </summary>
        public bool StopService()
        {
            try
            {
                ServiceController.Refresh();
                if (ServiceController.Status == ServiceControllerStatus.Running)
                {
                    ServiceController.Stop();
                    ServiceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                }
                return true;
            }
            catch (Exception ex)
            {
                Utils.WriteException(ex);
            }
            return false;
        }

        /// <summary>
        /// Windows service restart
        /// </summary>
        /// <returns>true if the service restarted, false otherwise</returns>
        public bool RestartService()
        {
            if (!StopService())
            {
                return false;
            }
            if (!StartService())
            {
                return false;
            }
            return true;
        }

        #endregion
    }
}
