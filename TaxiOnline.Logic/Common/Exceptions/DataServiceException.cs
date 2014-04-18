using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiOnline.Logic.Common.Exceptions
{
    public class DataServiceException : Exception
    {
        public DataServiceException(Exception innerException)
            : base(innerException.Message, innerException)
        {

        }
    }
}
