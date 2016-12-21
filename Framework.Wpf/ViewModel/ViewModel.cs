using System;
using System.Reflection;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Input;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;

namespace Framework.Wpf
{
    public class ViewModel : INotifyPropertyChanged
    {
        #region Public Properties

        public Window OwnerWindow { get; set; }

        public UserControl OwnerView { get; set; }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged(() => IsBusy);
                }
            }
        }

        private Dictionary<string, ICommand> _commands;
        public Dictionary<string, ICommand> Commands
        {
            get { return _commands ?? (_commands = new Dictionary<string, ICommand>()); }
        }

        #endregion

        #region Constructor

        public ViewModel()
        {
        }

        #endregion

        #region Public Methods

        public ICommand RegisterCommand(string commandKey, Action<object> executeMethod, Func<object, bool> canExecuteMethod = null)
        {
            var command = new RelayCommand(executeMethod, canExecuteMethod);
            Commands.Add(commandKey, command);
            return command;
        }

        public ICommand RegisterCommand(Action<object> executeMethod, Func<object, bool> canExecuteMethod = null)
        {
            var commandKey = executeMethod.Method.Name;
            return RegisterCommand(commandKey, executeMethod, canExecuteMethod);
        }

        #endregion

        #region Virtual Methods

        public virtual void Initialize()
        {
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected void OnPropertyChanged<T>(Expression<Func<T>> propertyExpresion)
        {
            MemberExpression memberExpression = (MemberExpression)propertyExpresion.Body;
            OnPropertyChanged(memberExpression.Member.Name);
        }

        #endregion
    }
}
