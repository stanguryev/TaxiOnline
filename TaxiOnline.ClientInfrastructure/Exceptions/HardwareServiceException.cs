using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ClientInfrastructure.Exceptions.Enums;

namespace TaxiOnline.ClientInfrastructure.Exceptions
{
   public class HardwareServiceException : Exception
    {
        private readonly HardwareServiceErrors _errorType;

        public HardwareServiceErrors ErrorType
        {
            get { return _errorType; }
        }

        public HardwareServiceException(HardwareServiceErrors errorType)
        {
            _errorType = errorType;
        }
    }
}
