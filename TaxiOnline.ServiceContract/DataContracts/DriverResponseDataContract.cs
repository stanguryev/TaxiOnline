using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TaxiOnline.ServiceContract.DataContracts
{
    [DataContract]
    public class DriverResponseDataContract
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public Guid DriverId { get; set; }

        [DataMember]
        public Guid RequestId { get; set; }

        [DataMember]
        public bool IsCanceled { get; set; }
    }
}
