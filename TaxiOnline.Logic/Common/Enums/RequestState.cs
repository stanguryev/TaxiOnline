using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiOnline.Logic.Common.Enums
{
    public enum RequestState
    {
        Created,
        Confirming,
        Comfirmed,
        ConfirmFailed,
        Canceling,
        CancelFailed,
        Canceled
    }
}
