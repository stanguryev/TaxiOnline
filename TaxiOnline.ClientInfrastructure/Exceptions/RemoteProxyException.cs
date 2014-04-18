using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ClientInfrastructure.Exceptions.Enums;

namespace TaxiOnline.ClientInfrastructure.Exceptions
{
    public class RemoteProxyException : Exception
    {
        private readonly RemoteProxyErrors _errorType;

        public RemoteProxyErrors ErrorType
        {
            get { return _errorType; }
        }

        public RemoteProxyException(RemoteProxyErrors errorType)
        {
            _errorType = errorType;
        }
    }
}
