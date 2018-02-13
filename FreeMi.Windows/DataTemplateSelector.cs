using FreeMi.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace FreeMi.Windows
{
    /// <summary>
    /// DataTemplate selector
    /// </summary>
    public class DataTemplateSelector : System.Windows.Controls.DataTemplateSelector
    {
        /// <summary>
        /// Returns a DataTemplate
        /// </summary>
        /// <param name="item">The data object for which to select the template.</param>
        /// <param name="container">The data-bound object.</param>
        /// <returns>Returns a DataTemplate or null</returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate dataTemplate = null;
            if (container is FrameworkElement && item is ITreeViewItemViewModel)
            {
                var viewModelTypeName = item.GetType().Name;
                dataTemplate = ((FrameworkElement)container).TryFindResource(viewModelTypeName.Substring(0, viewModelTypeName.IndexOf("ViewModel")) + "EditTemplate") as DataTemplate;
                if (dataTemplate == null)
                {
                    dataTemplate = ((FrameworkElement)container).TryFindResource("DefaultEditTemplate") as DataTemplate;
                }
            }
            return dataTemplate;
        }
    }
}
