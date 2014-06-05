using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxiOnline.MapEngine.Android.Cache
{
    public class DataWrapper<TData> : Java.Lang.Object
    {
        private readonly TData _data;

        public TData Data
        {
            get { return _data; }
        }

        public DataWrapper(TData data)
        {
            _data = data;
        }
    }
}
