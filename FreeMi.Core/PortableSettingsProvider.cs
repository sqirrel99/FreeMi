//http://www.codeproject.com/KB/vb/CustomSettingsProvider.aspx

using FreeMi.Core;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Text;
using System.Xml;

namespace FreeMi.Core
{
    /// <summary>
    /// PortableSettingsProvider
    /// </summary>
    public class PortableSettingsProvider : SettingsProvider
    {
        string _fileName;

        /// <summary>
        /// Initializes a new instance of PortableSettingsProvider class
        /// </summary>
        /// <param name="fileName">fileName</param>
        public PortableSettingsProvider(string fileName)
        {
            _fileName = fileName;
        }

        const string SETTINGSROOT = "Settings";
        //XML Root Node

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="col">NameValueCollection</param>
        public override void Initialize(string name, NameValueCollection col)
        {
            base.Initialize(this.ApplicationName, col);
        }

        /// <summary>
        /// Gets the application name
        /// </summary>
        public override string ApplicationName
        {
            get
            {
                if (Application.ProductName.Trim().Length > 0)
                {
                    return Application.ProductName;
                }
                else
                {
                    FileInfo fi = new FileInfo(Application.ExecutablePath);
                    return fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);
                }
            }
            set { }
            //Do nothing
        }

        /// <summary>
        /// Gets the provider name
        /// </summary>
        public override string Name
        {
            get { return "PortableSettingsProvider"; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual string GetAppSettingsPath()
        {
            //Used to determine where to store the settings
            System.IO.FileInfo fi = new System.IO.FileInfo(Application.ExecutablePath);
            return fi.DirectoryName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual string GetAppSettingsFilename()
        {
            //Used to determine the filename to store the settings
            //return ApplicationName + ".settings";
            return _fileName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="propvals"></param>
        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection propvals)
        {
            //Iterate through the settings to be stored
            //Only dirty settings are included in propvals, and only ones relevant to this provider
            foreach (SettingsPropertyValue propval in propvals)
            {
                SetValue(propval);
            }

            try
            {
                SettingsXML.Save(Path.Combine(GetAppSettingsPath(), GetAppSettingsFilename()));
            }
            catch (Exception)
            {
            }
            //Ignore if cant save, device been ejected
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="props"></param>
        /// <returns></returns>
        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection props)
        {
            //Create new collection of values
            SettingsPropertyValueCollection values = new SettingsPropertyValueCollection();

            //Iterate through the settings to be retrieved
            foreach (SettingsProperty setting in props)
            {
                SettingsPropertyValue value = new SettingsPropertyValue(setting);
                value.IsDirty = false;
                value.SerializedValue = GetValue(setting);
                values.Add(value);
            }
            return values;
        }

        private XmlDocument _settingsXML = null;

        private XmlDocument SettingsXML
        {
            get
            {
                //If we dont hold an xml document, try opening one.  
                //If it doesnt exist then create a new one ready.
                if (_settingsXML == null)
                {
                    _settingsXML = new XmlDocument();

                    try
                    {
                        _settingsXML.Load(Path.Combine(GetAppSettingsPath(), GetAppSettingsFilename()));
                    }
                    catch (Exception)
                    {
                        //Create new document
                        XmlDeclaration dec = _settingsXML.CreateXmlDeclaration("1.0", Encoding.UTF8.BodyName, string.Empty);
                        _settingsXML.AppendChild(dec);

                        XmlNode nodeRoot = default(XmlNode);

                        nodeRoot = _settingsXML.CreateNode(XmlNodeType.Element, SETTINGSROOT, "");
                        _settingsXML.AppendChild(nodeRoot);
                    }
                }

                return _settingsXML;
            }
        }

        private string GetValue(SettingsProperty setting)
        {
            string ret = "";

            try
            {
                if (IsRoaming(setting))
                {
                    ret = SettingsXML.SelectSingleNode(SETTINGSROOT + "/" + setting.Name).InnerXml;
                }
                else
                {
                    ret = SettingsXML.SelectSingleNode(SETTINGSROOT + "/" + Environment.MachineName + "/" + setting.Name).InnerXml;
                }
            }

            catch (Exception)
            {
                if ((setting.DefaultValue != null))
                {
                    ret = setting.DefaultValue.ToString();
                }
                else
                {
                    ret = "";
                }
            }

            return ret;
        }

        private void SetValue(SettingsPropertyValue propVal)
        {

            XmlElement machineNode = default(XmlElement);
            XmlElement settingNode = default(XmlElement);

            //Determine if the setting is roaming.
            //If roaming then the value is stored as an element under the root
            //Otherwise it is stored under a machine name node 
            try
            {
                if (IsRoaming(propVal.Property))
                {
                    settingNode = (XmlElement)SettingsXML.SelectSingleNode(SETTINGSROOT + "/" + propVal.Name);
                }
                else
                {
                    settingNode = (XmlElement)SettingsXML.SelectSingleNode(SETTINGSROOT + "/" + Environment.MachineName + "/" + propVal.Name);
                }
            }
            catch (Exception)
            {
                settingNode = null;
            }

            //Check to see if the node exists, if so then set its new value
            if ((settingNode != null))
            {
                SetInnerXml(settingNode, propVal);
            }
            else
            {
                if (IsRoaming(propVal.Property))
                {
                    //Store the value as an element of the Settings Root Node
                    settingNode = SettingsXML.CreateElement(propVal.Name);
                    SetInnerXml(settingNode, propVal);
                    SettingsXML.SelectSingleNode(SETTINGSROOT).AppendChild(settingNode);
                }
                else
                {
                    //Its machine specific, store as an element of the machine name node,
                    //creating a new machine name node if one doesnt exist.
                    try
                    {
                        machineNode = (XmlElement)SettingsXML.SelectSingleNode(SETTINGSROOT + "/" + Environment.MachineName);
                    }
                    catch (Exception)
                    {
                        machineNode = SettingsXML.CreateElement(Environment.MachineName);
                        SettingsXML.SelectSingleNode(SETTINGSROOT).AppendChild(machineNode);
                    }

                    if (machineNode == null)
                    {
                        machineNode = SettingsXML.CreateElement(Environment.MachineName);
                        SettingsXML.SelectSingleNode(SETTINGSROOT).AppendChild(machineNode);
                    }

                    settingNode = SettingsXML.CreateElement(propVal.Name);
                    SetInnerXml(settingNode, propVal);
                    machineNode.AppendChild(settingNode);
                }
            }
        }

        private void SetInnerXml(XmlElement settingNode, SettingsPropertyValue propVal)
        {
            settingNode.InnerXml = propVal.SerializedValue.ToString();
            if (settingNode.FirstChild is XmlDeclaration)
            {
                settingNode.RemoveChild(settingNode.FirstChild);
            }
        }

        private bool IsRoaming(SettingsProperty prop)
        {
            //Determine if the setting is marked as Roaming
            foreach (DictionaryEntry d in prop.Attributes)
            {
                Attribute a = (Attribute)d.Value;
                if (a is System.Configuration.SettingsManageabilityAttribute)
                {
                    return true;
                }
            }
            return false;
        }
    }
}