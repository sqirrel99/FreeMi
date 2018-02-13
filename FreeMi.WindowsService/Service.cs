using FreeMi.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;

namespace FreeMi.WindowsService
{
    /// <summary>
    /// Windows service
    /// </summary>
    public partial class Service : ServiceBase
    {
         #region Constructor

        /// <summary>
        /// Initializes a new instance of Service class
        /// </summary>
        public Service()
        {
            InitializeComponent();
            ServiceName = Application.WindowsServiceName;
        }

        #endregion

        #region Properties

        private Device _device;
        private Device Device
        {
            get
            {
                if (_device == null)
                {
                    _device = new Device();
                }
                return _device;
            }
        }

        #endregion

        #region Méthodes

        /// <summary>
        /// Method called on service start
        /// </summary>
        /// <param name="args">arguments</param>
        protected override void OnStart(string[] args)
        {
            Device.Start();
        }

        /// <summary>
        /// Method called on service stop
        /// </summary>
        protected override void OnStop()
        {
            try
            {
                Device.Dispose();
            }
            finally
            {
                _device = null;
            }
        }

        private static bool IsServiceInstalled()
        {
            return ServiceController.GetServices().Any(s => s.ServiceName == Application.WindowsServiceName);
        }

        /// <summary>
        /// Install the service
        /// </summary>
        public static void Install()
        {
            if (IsServiceInstalled())
            {
                Uninstall();
            }

            ManagedInstallerClass.InstallHelper(new string[] { "/LogFile", Assembly.GetExecutingAssembly().Location });
            
            var sc = new ServiceController(Application.WindowsServiceName);
            if (sc.Status == ServiceControllerStatus.Stopped)
            {
                sc.Start();
            }
        }

        /// <summary>
        /// Uninstall the service
        /// </summary>
        public static void Uninstall()
        {
            ManagedInstallerClass.InstallHelper(new string[] { "/u", "/LogFile", Assembly.GetExecutingAssembly().Location });
        } 

        #endregion
    }
}
