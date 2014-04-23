using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TaxiOnline.ServiceContract.DataContracts
{
    [DataContract]
    public class DriverAuthenticationRequestDataContract : AuthenticationRequestDataContract
    {
        [DataMember]
        public string PersonName { get; set; }

        [DataMember]
        public string CarColor { get; set; }

        [DataMember]
        public string CarBrand { get; set; }

        [DataMember]
        public string CarNumber { get; set; }
    }
}
