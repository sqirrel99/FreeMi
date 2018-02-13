using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace FreeMi.Windows
{
    /// <summary>
    /// Generic RelayCommand
    /// </summary>
    class RelayCommand<T> : ICommand
    {
        #region Events

        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute
        /// </summary>
        public event EventHandler CanExecuteChanged;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the RelayCommand class that can always execute
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <exception cref="ArgumentNullException">If the execute argument is null</exception>
        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the RelayCommand class
        /// </summary>
        /// <param name="execute">The execution logic</param>
        /// <param name="canExecute">The execution status logic</param>
        /// <exception cref="ArgumentNullException">If the execute argument is null</exception>
        public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            ExecuteAction = execute;
            CanExecuteFunc = canExecute;
        }

        #endregion

        #region Properties

        private Action<T> ExecuteAction { get; set; }

        private Func<T, bool> CanExecuteFunc { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">This parameter will always be ignored.</param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc == null ? true : CanExecuteFunc(parameter == null ? default(T) : (T)parameter);
        }

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged" /> event.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked
        /// </summary>
        /// <param name="parameter">This parameter will always be ignored</param>
        public void Execute(object parameter)
        {
            if (ExecuteAction != null)
            {
                ExecuteAction(parameter == null ? default(T) : (T)parameter);
            }
        }

        #endregion
    }
}
