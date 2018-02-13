using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace FreeMi.Windows
{
    /// <summary>
    /// Static class for ResourceKey attached property
    /// </summary>
    public static class ImageSource
    {
        private static void ResourceKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) { return; }
            
            var element = d as Image;
            if (element == null) { return; }

            element.SetResourceReference(Image.SourceProperty, e.NewValue);
        }

        /// <summary>
        /// ResourceKey attached property
        /// </summary>
        public static readonly DependencyProperty ResourceKeyProperty = DependencyProperty.RegisterAttached("ResourceKey",
            typeof(string),
            typeof(ImageSource),
            new PropertyMetadata(String.Empty, ResourceKeyChanged));

        /// <summary>
        /// Gets the source resource key
        /// </summary>
        /// <param name="element">image element</param>
        /// <returns>the source resource key</returns>
        public static string GetResourceKey(Image element)
        {
            return element.GetValue(ResourceKeyProperty) as string;
        }

        /// <summary>
        /// Sets the source resource key
        /// </summary>
        /// <param name="element">image element</param>
        /// <param name="value">the source resource key</param>
        public static void SetResourceKey(Image element, string value)
        {
            element.SetValue(ResourceKeyProperty, value);
        }        
    }
}
