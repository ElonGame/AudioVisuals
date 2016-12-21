using System;
using System.Reflection;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Input;
using System.Collections.Generic;

namespace Framework.Wpf
{
    public class ItemViewModel<T> : ViewModel
    {
        #region Public Properties

        public T Model { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(() => IsSelected);
                }
            }
        }

        #endregion

        #region Constructors

        public ItemViewModel()
        {
        }

        public ItemViewModel(T model)
        {
            Model = model;
        }

        #endregion
    }
}
