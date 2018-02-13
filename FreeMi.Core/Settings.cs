using FreeMi.Core.Entries;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FreeMi.Core.Properties
{
    // This class allows you to handle specific events on the settings class:
    //  The SettingChanging event is raised before a setting's value is changed.
    //  The PropertyChanged event is raised after a setting's value is changed.
    //  The SettingsLoaded event is raised after the setting values are loaded.
    //  The SettingsSaving event is raised before the setting values are saved.
    public sealed partial class Settings
    {
        /// <summary>
        /// Initializes a new instance of Settings classs
        /// </summary>
        public Settings()
        {
            // // To add event handlers for saving and changing settings, uncomment the lines below:
            //
            // this.SettingChanging += this.SettingChangingEventHandler;
            //
            // this.SettingsSaving += this.SettingsSavingEventHandler;
            //

            var runningMode = Application.RunningMode;
            if (runningMode == RunningMode.Portable || runningMode == RunningMode.Service)
            {
                var portableSettingsProvider = new PortableSettingsProvider("user.config");
                Providers.Add(portableSettingsProvider);
                foreach (SettingsProperty prop in Properties)
                    prop.Provider = portableSettingsProvider;
            }
        }

        private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e)
        {
            // Add code to handle the SettingChangingEvent event here.
        }

        private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Add code to handle the SettingsSaving event here.
        }

        /// <summary>
        /// Gets or sets the list of the entries
        /// </summary>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public EntryCollection Entries
        {
            get { return ((EntryCollection)(this["Entries"])); }
            set { this["Entries"] = value; }
        }

        #region Methods

        /// <summary>
        /// Raises the System.Configuration.ApplicationSettingsBase.SettingsLoaded event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A System.Configuration.SettingsLoadedEventArgs that contains the event data.</param>
        protected override void OnSettingsLoaded(object sender, SettingsLoadedEventArgs e)
        {
            base.OnSettingsLoaded(sender, e);

            Settings settings = Settings.Default;
            if (settings.UpdateRequired)
            {
                settings.Upgrade();
                settings.UpdateRequired = false;
                if (String.IsNullOrWhiteSpace(settings.UDN))
                {
                    settings.UDN = Guid.NewGuid().ToString();
                }
                if (settings.Entries == null)
                {
                    var modeService = Application.RunningMode == RunningMode.Service;
                    settings.Entries = new EntryCollection();
                    settings.Entries.Add(new Folder()
                        {
                            Label = Resources.Music,
                            Path = Environment.GetFolderPath(modeService ? Environment.SpecialFolder.CommonMusic : Environment.SpecialFolder.MyMusic),
                            MediaKind = MediaKind.Audio
                        });
                    settings.Entries.Add(new Folder()
                        {
                            Label = Resources.Pictures,
                            Path = Environment.GetFolderPath(modeService ? Environment.SpecialFolder.CommonPictures : Environment.SpecialFolder.MyPictures),
                            MediaKind = MediaKind.Image
                        });
                    settings.Entries.Add(new Folder()
                        {
                            Label = Resources.Videos,
                            Path = Environment.GetFolderPath(modeService ? Environment.SpecialFolder.CommonVideos : Environment.SpecialFolder.MyVideos),
                            MediaKind = MediaKind.Video
                        });
                    settings.Entries.Add(new Podcast()
                        {
                            Label = Resources.FreeMiPodcasts,
                            Path = "http://freemiupnp.fr/download/podcasts.xml"
                        });
                }

                settings.Save();
            }
        }

        #endregion
    }
}
