using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiOnline.ClientInfrastructure.Exceptions.Enums
{
    public enum RemoteProxyErrors
    {
        Timeout,
        FailedToReconnect,
        IsConnecting,
        ConnectionError,
        ConnectionIsClosedError,
        ConnectionIsClosingError,
        ConnectionIsNotEstablishedError,
        ConnectionWasChangedError,
        SessionIsClosedError
    }
}
