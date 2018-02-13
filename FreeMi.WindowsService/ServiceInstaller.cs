using FreeMi.Core;
using FreeMi.WindowsService.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace FreeMi.WindowsService
{
    /// <summary>
    /// Service installer
    /// </summary>
    [RunInstaller(true)]
    public class ServiceInstaller : Installer
    {
        /// <summary>
        /// Initializes a new instance of ServiceInstaller
        /// </summary>
        public ServiceInstaller()
        {
            var process = new ServiceProcessInstaller();
            process.Account = ServiceAccount.LocalSystem;

            var service = new System.ServiceProcess.ServiceInstaller();
            service.ServiceName = Application.WindowsServiceName;
            service.DisplayName = String.Format(Resources.Service, Application.ProductName);          
            service.StartType = ServiceStartMode.Automatic;

            Installers.Add(process);
            Installers.Add(service);
        }
    }
}
