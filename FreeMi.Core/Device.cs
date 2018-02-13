using FreeMi.Core.Entries;
using FreeMi.Core.Properties;
using OpenSource.UPnP;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Timers;

namespace FreeMi.Core
{
    /// <summary>
    /// Device class
    /// </summary>
    public class Device : IDisposable
    {
        #region Constantes

        private const string CONTENT_DIRECTORY_SERVICE_ID = "ContentDirectory";
        private const string CONNECTION_MANAGER_SERVICE_ID = "ConnectionManager";

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the Device class
        /// </summary>
        public Device()
            : base()
        {
            UPnPDevice device = UPnPDevice.CreateRootDevice(1800, 1.0, "\\");
            device.Icon = Resources.Generic_Device120;
            device.Icon2 = Resources.Generic_Device48;
            device.Manufacturer = "FreeMi";
            device.ManufacturerURL = "http://freemiupnp.fr/";
            device.ModelName = "FreeMi UPnP Media Server";
            device.ModelDescription = "UPnP/AV 1.0 Compliant Media Server for Freebox";
            device.ModelNumber = "2.0";
            device.HasPresentation = false;
            device.StandardDeviceType = "MediaServer";
            device.SerialNumber = "{D6E1D58B-5E95-4E59-A913-5F79B09CF93E}";
            device.ModelURL = new Uri("http://freemiupnp.fr/");
            UPnPDevice = device;

            ConnectionManager connectionManager = new ConnectionManager(CONNECTION_MANAGER_SERVICE_ID);
            device.AddService(connectionManager);

            FreeMiContentDirectoryService contentDirectory = new FreeMiContentDirectoryService(CONTENT_DIRECTORY_SERVICE_ID);
            //contentDirectory.BeforeShutdown += new EventHandler(BeforeShutdown);
            ContentDirectoryService = contentDirectory;
            device.AddService(contentDirectory);
            device.AddVirtualDirectory("FreeMi", null, contentDirectory.PacketCallBack);

            // Setting the initial value of evented variables
            contentDirectory.Evented_SystemUpdateID = 0;

            Shutdown.BeforeShutdown += Shutdown_BeforeShutdown;
            NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;
        }

        #endregion

        #region Properties

        private UPnPDevice UPnPDevice
        {
            get;
            set;
        }

        private FreeMiContentDirectoryService ContentDirectoryService
        {
            get;
            set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Start the device
        /// </summary>
        public void Start()
        {
            try
            {
                Stop();
            }
            catch (Exception ex)
            {
                Utils.WriteException(ex);
            }
            var settings = Settings.Default;
            NetworkInfo.IPv6Enabled = settings.IPv6Enabled;
            var portNumber = settings.PortNumber;
            string friendlyName = settings.FriendlyName;
            UPnPDevice device = UPnPDevice;
            try
            {
                device.FriendlyName = String.IsNullOrWhiteSpace(friendlyName) ?
                    String.Format(Application.RunningMode == RunningMode.Service ? "FreeMi UPnP ({0})" : "FreeMi UPnP ({0} : {1})", 
                    Environment.MachineName, Environment.UserName) : friendlyName;
            }
            catch (Exception ex)
            {
                Utils.WriteException(ex);
                device.FriendlyName = Application.ProductName;
            }
            device.UniqueDeviceName = Settings.Default.UDN;
            device.StartDevice(portNumber);
        }

        /// <summary>
        /// Stop the device
        /// </summary>
        public void Stop()
        {
            try
            {
                UPnPDevice.StopDevice();
            }
            catch (Exception ex)
            {
                Utils.WriteException(ex);
            }      
        }

        /// <summary>
        /// Restart the device
        /// </summary>
        public bool Restart()
        {
            try
            {
                Stop();
                UPnPDevice.RefreshNetworkInfo();
                Start();
                return true;
            }
            catch (Exception ex)
            {
                Utils.WriteException(ex);
            }
            return false;
        }

        private void Advertise()
        {
            try
            {
                UPnPDevice.Advertise();
            }
            catch (Exception ex)
            {
                Utils.WriteException(ex);
            }
        }        

        private void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            if (e.IsAvailable)
            {
                Restart();
            }
        }

        private void Shutdown_BeforeShutdown(object sender, EventArgs e)
        {
            Stop();
        }

        #endregion

        #region IDisposable Membres

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Stop();
            Shutdown.BeforeShutdown -= Shutdown_BeforeShutdown;
            NetworkChange.NetworkAvailabilityChanged -= NetworkChange_NetworkAvailabilityChanged;
        }

        #endregion
    }
}
