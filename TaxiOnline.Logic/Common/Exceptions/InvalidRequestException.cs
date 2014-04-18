using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.Logic.Common.Enums;

namespace TaxiOnline.Logic.Common.Exceptions
{
    public class InvalidRequestException : InvalidOperationException
    {
        private readonly RequestErrors _error;

        public RequestErrors Error
        {
            get { return _error; }
        }

        public InvalidRequestException(RequestErrors error)
        {
            _error = error;
        }
    }
}
