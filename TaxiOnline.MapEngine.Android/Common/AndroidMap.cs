using Android.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.MapEngine.Common;

namespace TaxiOnline.MapEngine.Android.Common
{
    public class AndroidMap : MapBase<byte[], Bitmap>
    {
        public AndroidMap()
            : base(new AndroidMapSupplyFactory())
        {

        }
    }
}
