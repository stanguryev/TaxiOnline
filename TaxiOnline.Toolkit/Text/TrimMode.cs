using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxiOnline.Toolkit.Text
{
    [Flags]
    public enum TrimMode
    {
        None = 0x0,
        Once = 0x1,
        WholeEntry = 0x2
    }
}
