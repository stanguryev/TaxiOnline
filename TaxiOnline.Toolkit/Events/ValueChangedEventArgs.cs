using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxiOnline.Toolkit.Events
{
    public class ValueChangedEventArgs<T> : EventArgs
    {
        private readonly T _oldValue;
        private readonly T _newValue;

        public T OldValue
        {
            get { return _oldValue; }
        }

        public T NewValue
        {
            get { return _newValue; }
        } 
        
        public ValueChangedEventArgs(T oldValue, T newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }
    }
}
