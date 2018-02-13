using FreeMi.WindowsService.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Windows;

namespace FreeMi.WindowsService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// <param name="args">arguments</param>
        /// </summary>
        static void Main(string[] args)
        {
            if (args != null && args.Any())
            {
                bool silentMode = false;
                foreach (var arg in args)
                {
                    if (!String.IsNullOrWhiteSpace(arg) && arg.ToUpper() == "/S")
                    {
                        silentMode = true;
                        break;
                    }
                }

                try
                {
                    foreach (var arg in args)
                    {
                        if (!String.IsNullOrWhiteSpace(arg))
                        {
                            switch (arg.ToUpper())
                            {
                                case "/I":
                                    Service.Install();
                                    if (!silentMode)
                                    {
                                        MessageBox.Show(Resources.SuccessfulInstall, FreeMi.Core.Application.ProductNameAndVersion, MessageBoxButton.OK, MessageBoxImage.Information);
                                    }
                                    return;
                                case "/U":
                                    Service.Uninstall();
                                    if (!silentMode)
                                    {
                                        MessageBox.Show(Resources.SuccessfulUninstall, FreeMi.Core.Application.ProductNameAndVersion, MessageBoxButton.OK, MessageBoxImage.Information);
                                    }
                                    return;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (!silentMode)
                    {
                        MessageBox.Show(ex.Message, FreeMi.Core.Application.ProductNameAndVersion, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    return;
                }
            }

            ServiceBase.Run(new Service());
        }
    }
}
