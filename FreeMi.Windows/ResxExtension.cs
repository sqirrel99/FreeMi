using FreeMi.UI.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace FreeMi.Windows
{
    /// <summary>
    /// MarkupExtension for the resources file
    /// </summary>
    public class ResxExtension : MarkupExtension
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of ResxExtension class
        /// </summary>
        public ResxExtension()
            : base()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the resource key
        /// </summary>
        public string Key { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns an object that is set as the value of the target property for this markup extension
        /// </summary>
        /// <param name="serviceProvider">Object that can provide services for the markup extension</param>
        /// <returns>an object that is set as the value of the target property for this markup extension</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (!String.IsNullOrEmpty(Key))
            {
                try
                {
                    // Create a strongly typed resource manager instance
                    var result = Resources.ResourceManager.GetString(Key);
                    if (!String.IsNullOrEmpty(result))
                    {
                        return result;
                    }
                }
                catch (Exception)
                {
                    // Purposely do nothing here to allow the call to fall through
                }
            }
            return Key;
        }

        #endregion
    }
}
