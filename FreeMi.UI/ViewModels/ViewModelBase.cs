using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace FreeMi.UI.ViewModels
{
    /// <summary>
    /// Base class for ViewModels with no WPF dependences
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        #region Events

        /// <summary>
        /// Occurs when a property value changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Méthodes

        /// <summary>
        /// Raises the PropertyChanged event
        /// </summary>
        /// <param name="propertyName">property name</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }                 
        }

        /// <summary>
        /// Extracts the name of a property from an expression
        /// </summary>
        /// <typeparam name="T">The type of the property</typeparam>
        /// <param name="propertyExpression">An expression returning the property's name</param>
        /// <returns>The name of the property returned by the expression</returns>
        /// <exception cref="ArgumentNullException">If the expression is null</exception>
        /// <exception cref="ArgumentException">If the expression does not represent a property</exception>
        protected string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException("propertyExpression");
            }

            var body = propertyExpression.Body as MemberExpression;

            if (body == null)
            {
                throw new ArgumentException("Invalid argument", "propertyExpression");
            }

            var property = body.Member as PropertyInfo;

            if (property == null)
            {
                throw new ArgumentException("Argument is not a property", "propertyExpression");
            }

            return property.Name;
        }

        /// <summary>
        /// Raises the PropertyChanged event if needed
        /// </summary>
        /// <typeparam name="T">The type of the property that changed</typeparam>
        /// <param name="propertyExpression">An expression identifying the property that changed</param>
        protected virtual void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            OnPropertyChanged(GetPropertyName(propertyExpression));
        }

        #endregion
    }
}
