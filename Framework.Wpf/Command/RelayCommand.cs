using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;

namespace Framework.Wpf
{
    /// <summary>
    /// A command whose sole purpose is to 
    /// relay its functionality to other
    /// objects by invoking delegates. The
    /// default return value for the CanExecute
    /// method is 'true'.
    /// </summary>
    public class RelayCommand : ICommand, INotifyPropertyChanged
    {
        #region Private Member Variables

        private Func<object, bool> _canExecuteDelegate;
        private bool _didExecute;
        private Action<object> _executeDelegate;
        private event PropertyChangedEventHandler _propertyChanged;

        #endregion

        #region Public Properties

        public Func<object, bool> CanExecuteDelegate
        {
            get { return _canExecuteDelegate; }
            set { _canExecuteDelegate = value; }
        }

        public bool DidExecute
        {
            get { return _didExecute; }
            set
            {
                if (_didExecute != value)
                {
                    _didExecute = value;
                    OnPropertyChanged("DidExecute");
                }
            }
        }

        bool _didSucceed;
        public bool DidSucceed
        {
            get { return _didSucceed; }
            set
            {
                if (_didSucceed != value)
                {
                    _didSucceed = value;
                    OnPropertyChanged("DidSucceed");
                }
            }
        }

        public Action<object> ExecuteDelegate
        {
            get { return _executeDelegate; }
            set { _executeDelegate = value; }
        }

        #endregion

        #region Constructors

        public RelayCommand()
        {
        }

        /// <summary>
        /// Creates a new command that can always execute.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        public RelayCommand(Action<object> execute) : this(execute, null)
        {
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            if ( execute == null )
            {
                throw new ArgumentNullException( "execute" );
            }

            _executeDelegate = execute;
            _canExecuteDelegate = canExecute;
        }

        #endregion

        #region Protected Methods

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = _propertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        #endregion

        #region ICommand Methods

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            _didExecute = false;
            return _canExecuteDelegate == null ? true : _canExecuteDelegate(parameter);
        }

        public void Execute(object parameter)
        {
            if (_executeDelegate != null)
            {
                _executeDelegate(parameter);
                DidExecute = true;
            }
        }

        #endregion

        #region ICommand Events

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        #endregion

        #region INotifyPropertyChanged Events

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _propertyChanged += value; }
            remove { _propertyChanged -= value; }
        }

        #endregion
    }
}