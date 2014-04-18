using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxiOnline.Toolkit.Events
{
    /// <summary>
    /// класс для упрощения описания события, передающего параметр
    /// </summary>
    /// <typeparam name="T">тип параметра</typeparam>
    public class ValueEventArgs<T> : EventArgs
    {
        private readonly T _value;

        /// <summary>
        /// значение параметра
        /// </summary>
        public T Value
        {
            get { return _value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">передаваемое значение параметра</param>
        public ValueEventArgs(T value)
        {
            _value = value;
        }
    }
}
